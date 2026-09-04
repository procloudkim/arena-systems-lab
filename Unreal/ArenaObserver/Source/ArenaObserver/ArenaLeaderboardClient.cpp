#include "ArenaLeaderboardClient.h"

#include "Dom/JsonObject.h"
#include "HAL/PlatformTime.h"
#include "IPAddress.h"
#include "Serialization/JsonReader.h"
#include "Serialization/JsonSerializer.h"
#include "SocketSubsystem.h"
#include "Sockets.h"

namespace
{
	constexpr int32 ProtocolVersion = 1;
	constexpr int32 PrefixBytes = 4;
	constexpr int32 Port = 7777;
	constexpr double RequestTimeoutSeconds = 3.0;
	constexpr ANSICHAR RequestJson[] = "{\"version\":1,\"type\":\"get_leaderboard\",\"limit\":5}";

	FArenaLeaderboardResult Failure(const TCHAR* Status)
	{
		FArenaLeaderboardResult Result;
		Result.Status = Status;
		return Result;
	}

	bool IsValidPlayerId(const FString& PlayerId)
	{
		if (PlayerId.IsEmpty() || PlayerId.Len() > 32)
		{
			return false;
		}

		for (const TCHAR Character : PlayerId)
		{
			const bool bAllowed = (Character >= TEXT('a') && Character <= TEXT('z'))
				|| (Character >= TEXT('A') && Character <= TEXT('Z'))
				|| (Character >= TEXT('0') && Character <= TEXT('9'))
				|| Character == TEXT('_')
				|| Character == TEXT('-');
			if (!bAllowed)
			{
				return false;
			}
		}

		return true;
	}

	bool TryGetStrictInt(const TSharedPtr<FJsonObject>& Object, const TCHAR* Field, int32& OutValue)
	{
		if (!Object->HasTypedField<EJson::Number>(Field))
		{
			return false;
		}

		double Number = 0.0;
		if (!Object->TryGetNumberField(Field, Number)
			|| !FMath::IsFinite(Number)
			|| Number < static_cast<double>(MIN_int32)
			|| Number > static_cast<double>(MAX_int32)
			|| Number != FMath::FloorToDouble(Number))
		{
			return false;
		}

		OutValue = static_cast<int32>(Number);
		return true;
	}

	FTimespan Remaining(double Deadline)
	{
		return FTimespan::FromSeconds(FMath::Max(0.0, Deadline - FPlatformTime::Seconds()));
	}

	bool WaitFor(FSocket& Socket, ESocketWaitConditions::Type Condition, double Deadline, FString& OutError)
	{
		const FTimespan TimeLeft = Remaining(Deadline);
		if (TimeLeft <= FTimespan::Zero() || !Socket.Wait(Condition, TimeLeft))
		{
			OutError = Socket.GetConnectionState() == SCS_ConnectionError
				? TEXT("connection_io_error")
				: TEXT("request_timeout");
			return false;
		}

		return true;
	}

	bool SendExactly(FSocket& Socket, const TArray<uint8>& Data, double Deadline, FString& OutError)
	{
		int32 Offset = 0;
		while (Offset < Data.Num())
		{
			if (!WaitFor(Socket, ESocketWaitConditions::WaitForWrite, Deadline, OutError))
			{
				return false;
			}

			int32 Sent = 0;
			if (!Socket.Send(Data.GetData() + Offset, Data.Num() - Offset, Sent) || Sent <= 0)
			{
				OutError = TEXT("connection_io_error");
				return false;
			}

			Offset += Sent;
		}

		return true;
	}

	bool ReceiveExactly(FSocket& Socket, uint8* Data, int32 Size, double Deadline, FString& OutError)
	{
		int32 Offset = 0;
		while (Offset < Size)
		{
			if (!WaitFor(Socket, ESocketWaitConditions::WaitForRead, Deadline, OutError))
			{
				return false;
			}

			int32 Received = 0;
			if (!Socket.Recv(Data + Offset, Size - Offset, Received) || Received <= 0)
			{
				OutError = TEXT("incomplete_frame");
				return false;
			}

			Offset += Received;
		}

		return true;
	}
}

TArray<uint8> FArenaLeaderboardClient::BuildRequestFrame()
{
	constexpr int32 PayloadLength = UE_ARRAY_COUNT(RequestJson) - 1;
	TArray<uint8> Frame;
	Frame.Reserve(PrefixBytes + PayloadLength);
	Frame.Add(static_cast<uint8>(PayloadLength >> 24));
	Frame.Add(static_cast<uint8>(PayloadLength >> 16));
	Frame.Add(static_cast<uint8>(PayloadLength >> 8));
	Frame.Add(static_cast<uint8>(PayloadLength));
	Frame.Append(reinterpret_cast<const uint8*>(RequestJson), PayloadLength);
	return Frame;
}

bool FArenaLeaderboardClient::ParseResponse(
	const TArray<uint8>& Payload,
	TArray<FArenaLeaderboardEntry>& OutEntries,
	FString& OutError)
{
	OutEntries.Reset();
	OutError = TEXT("invalid_response");
	if (Payload.IsEmpty() || Payload.Num() > MaxPayloadBytes)
	{
		return false;
	}

	FUtf8String Json = FUtf8String::ConstructFromPtrSize(
		reinterpret_cast<const UTF8CHAR*>(Payload.GetData()),
		Payload.Num());
	const TSharedRef<TJsonReader<UTF8CHAR>> Reader = TJsonReaderFactory<UTF8CHAR>::Create(MoveTemp(Json));
	TSharedPtr<FJsonObject> Root;
	if (!FJsonSerializer::Deserialize(Reader, Root) || !Root.IsValid())
	{
		return false;
	}

	int32 Version = 0;
	bool bOk = false;
	const TArray<TSharedPtr<FJsonValue>>* JsonEntries = nullptr;
	if (!TryGetStrictInt(Root, TEXT("version"), Version)
		|| Version != ProtocolVersion
		|| !Root->HasTypedField<EJson::Boolean>(TEXT("ok"))
		|| !Root->TryGetBoolField(TEXT("ok"), bOk)
		|| !bOk
		|| !Root->TryGetArrayField(TEXT("entries"), JsonEntries)
		|| JsonEntries == nullptr
		|| JsonEntries->Num() > LeaderboardLimit)
	{
		return false;
	}

	for (const TSharedPtr<FJsonValue>& JsonEntry : *JsonEntries)
	{
		const TSharedPtr<FJsonObject>* EntryObject = nullptr;
		if (!JsonEntry.IsValid() || !JsonEntry->TryGetObject(EntryObject)
			|| EntryObject == nullptr || !EntryObject->IsValid())
		{
			OutEntries.Reset();
			return false;
		}

		FArenaLeaderboardEntry Entry;
		if (!(*EntryObject)->HasTypedField<EJson::String>(TEXT("playerId"))
			|| !(*EntryObject)->TryGetStringField(TEXT("playerId"), Entry.PlayerId)
			|| !IsValidPlayerId(Entry.PlayerId)
			|| !TryGetStrictInt(*EntryObject, TEXT("score"), Entry.Score)
			|| Entry.Score < 0 || Entry.Score > MaxScore)
		{
			OutEntries.Reset();
			return false;
		}

		if (!OutEntries.IsEmpty())
		{
			const FArenaLeaderboardEntry& Previous = OutEntries.Last();
			const bool bWrongScoreOrder = Previous.Score < Entry.Score;
			const bool bWrongTieOrder = Previous.Score == Entry.Score
				&& Previous.PlayerId.Compare(Entry.PlayerId, ESearchCase::CaseSensitive) > 0;
			if (bWrongScoreOrder || bWrongTieOrder)
			{
				OutEntries.Reset();
				return false;
			}
		}

		for (const FArenaLeaderboardEntry& Existing : OutEntries)
		{
			if (Existing.PlayerId == Entry.PlayerId)
			{
				OutEntries.Reset();
				return false;
			}
		}

		OutEntries.Add(MoveTemp(Entry));
	}

	OutError.Reset();
	return true;
}

FArenaLeaderboardResult FArenaLeaderboardClient::Fetch()
{
	ISocketSubsystem* SocketSubsystem = ISocketSubsystem::Get(PLATFORM_SOCKETSUBSYSTEM);
	if (SocketSubsystem == nullptr)
	{
		return Failure(TEXT("socket_unavailable"));
	}

	TSharedRef<FInternetAddr> Address = SocketSubsystem->CreateInternetAddr();
	bool bValidAddress = false;
	Address->SetIp(TEXT("127.0.0.1"), bValidAddress);
	Address->SetPort(Port);
	if (!bValidAddress)
	{
		return Failure(TEXT("socket_unavailable"));
	}

	FUniqueSocket Socket = SocketSubsystem->CreateUniqueSocket(NAME_Stream, TEXT("ArenaObserver"));
	if (!Socket || !Socket->SetNonBlocking(true))
	{
		return Failure(TEXT("socket_unavailable"));
	}

	const double Deadline = FPlatformTime::Seconds() + RequestTimeoutSeconds;
	if (!Socket->Connect(*Address))
	{
		return Failure(TEXT("connection_failed"));
	}

	FString Error;
	if (!WaitFor(*Socket, ESocketWaitConditions::WaitForWrite, Deadline, Error)
		|| Socket->GetConnectionState() != SCS_Connected)
	{
		return Failure(Error.IsEmpty() ? TEXT("connection_failed") : *Error);
	}

	const TArray<uint8> RequestFrame = BuildRequestFrame();
	if (!SendExactly(*Socket, RequestFrame, Deadline, Error))
	{
		return Failure(*Error);
	}

	uint8 Prefix[PrefixBytes] = {};
	if (!ReceiveExactly(*Socket, Prefix, PrefixBytes, Deadline, Error))
	{
		return Failure(*Error);
	}

	const uint32 PayloadLength = static_cast<uint32>(Prefix[0]) << 24
		| static_cast<uint32>(Prefix[1]) << 16
		| static_cast<uint32>(Prefix[2]) << 8
		| static_cast<uint32>(Prefix[3]);
	if (PayloadLength == 0 || PayloadLength > MaxPayloadBytes)
	{
		return Failure(TEXT("invalid_frame_length"));
	}

	TArray<uint8> Payload;
	Payload.SetNumUninitialized(static_cast<int32>(PayloadLength));
	if (!ReceiveExactly(*Socket, Payload.GetData(), Payload.Num(), Deadline, Error))
	{
		return Failure(*Error);
	}

	FArenaLeaderboardResult Result;
	if (!ParseResponse(Payload, Result.Entries, Result.Status))
	{
		return Result;
	}

	Result.bSuccess = true;
	Result.Status = TEXT("Connected to 127.0.0.1:7777");
	return Result;
}
