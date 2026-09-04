#if WITH_DEV_AUTOMATION_TESTS

#include "ArenaLeaderboardClient.h"
#include "Misc/AutomationTest.h"
#include "Misc/CommandLine.h"
#include "Misc/Parse.h"

namespace
{
	TArray<uint8> Utf8(const ANSICHAR* Text)
	{
		TArray<uint8> Bytes;
		Bytes.Append(reinterpret_cast<const uint8*>(Text), FCStringAnsi::Strlen(Text));
		return Bytes;
	}
}

IMPLEMENT_SIMPLE_AUTOMATION_TEST(
	FArenaLeaderboardProtocolTest,
	"ArenaSystemsLab.ArenaObserver.Protocol",
	EAutomationTestFlags::EditorContext | EAutomationTestFlags::EngineFilter)

bool FArenaLeaderboardProtocolTest::RunTest(const FString& Parameters)
{
	const TArray<uint8> Frame = FArenaLeaderboardClient::BuildRequestFrame();
	const TArray<uint8> ExpectedRequest = Utf8("{\"version\":1,\"type\":\"get_leaderboard\",\"limit\":5}");
	if (!TestEqual(TEXT("Frame size"), Frame.Num(), ExpectedRequest.Num() + 4))
	{
		return false;
	}

	const int32 FramedLength = static_cast<int32>(Frame[0]) << 24
		| static_cast<int32>(Frame[1]) << 16
		| static_cast<int32>(Frame[2]) << 8
		| static_cast<int32>(Frame[3]);
	TestEqual(TEXT("Frame length prefix"), FramedLength, ExpectedRequest.Num());
	TestTrue(
		TEXT("Request payload"),
		Frame.Num() == ExpectedRequest.Num() + 4
			&& FMemory::Memcmp(Frame.GetData() + 4, ExpectedRequest.GetData(), ExpectedRequest.Num()) == 0);

	TArray<FArenaLeaderboardEntry> Entries;
	FString Error;
	const TArray<uint8> Valid = Utf8(
		"{\"version\":1,\"ok\":true,\"entries\":["
		"{\"playerId\":\"UnityPlayer\",\"score\":11},"
		"{\"playerId\":\"OtherPlayer\",\"score\":7}]}");
	TestTrue(TEXT("Valid response"), FArenaLeaderboardClient::ParseResponse(Valid, Entries, Error));
	TestEqual(TEXT("Entry count"), Entries.Num(), 2);
	if (Entries.Num() == 2)
	{
		TestEqual(TEXT("First player"), Entries[0].PlayerId, FString(TEXT("UnityPlayer")));
		TestEqual(TEXT("First score"), Entries[0].Score, 11);
	}

	const TArray<uint8> WrongOrder = Utf8(
		"{\"version\":1,\"ok\":true,\"entries\":["
		"{\"playerId\":\"A\",\"score\":1},{\"playerId\":\"B\",\"score\":2}]}");
	TestFalse(TEXT("Unsorted response"), FArenaLeaderboardClient::ParseResponse(WrongOrder, Entries, Error));

	const TArray<uint8> FractionalScore = Utf8(
		"{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"A\",\"score\":1.5}]}");
	TestFalse(TEXT("Fractional score"), FArenaLeaderboardClient::ParseResponse(FractionalScore, Entries, Error));

	const TArray<uint8> InvalidPlayer = Utf8(
		"{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"../bad\",\"score\":1}]}");
	TestFalse(TEXT("Invalid player ID"), FArenaLeaderboardClient::ParseResponse(InvalidPlayer, Entries, Error));

	const TArray<uint8> DuplicatePlayer = Utf8(
		"{\"version\":1,\"ok\":true,\"entries\":["
		"{\"playerId\":\"A\",\"score\":2},{\"playerId\":\"A\",\"score\":1}]}");
	TestFalse(TEXT("Duplicate player"), FArenaLeaderboardClient::ParseResponse(DuplicatePlayer, Entries, Error));

	TArray<uint8> Oversized;
	Oversized.SetNumZeroed(FArenaLeaderboardClient::MaxPayloadBytes + 1);
	TestFalse(TEXT("Oversized payload"), FArenaLeaderboardClient::ParseResponse(Oversized, Entries, Error));

	if (FParse::Param(FCommandLine::Get(), TEXT("ArenaObserverExpectServer")))
	{
		const FArenaLeaderboardResult Result = FArenaLeaderboardClient::Fetch();
		TestTrue(TEXT("Actual server query"), Result.bSuccess);
	}
	else if (FParse::Param(FCommandLine::Get(), TEXT("ArenaObserverExpectNoServer")))
	{
		const FArenaLeaderboardResult Result = FArenaLeaderboardClient::Fetch();
		TestFalse(TEXT("Unavailable server query"), Result.bSuccess);
		TestTrue(TEXT("Unavailable status"), !Result.Status.IsEmpty());
	}

	return true;
}

#endif
