#pragma once

#include "CoreMinimal.h"

struct FArenaLeaderboardEntry
{
	FString PlayerId;
	int32 Score = 0;
};

struct FArenaLeaderboardResult
{
	bool bSuccess = false;
	FString Status;
	TArray<FArenaLeaderboardEntry> Entries;
};

class FArenaLeaderboardClient
{
public:
	static constexpr int32 MaxPayloadBytes = 16 * 1024;
	static constexpr int32 MaxScore = 1'000'000;
	static constexpr int32 LeaderboardLimit = 5;

	static TArray<uint8> BuildRequestFrame();
	static bool ParseResponse(
		const TArray<uint8>& Payload,
		TArray<FArenaLeaderboardEntry>& OutEntries,
		FString& OutError);
	static FArenaLeaderboardResult Fetch();
};
