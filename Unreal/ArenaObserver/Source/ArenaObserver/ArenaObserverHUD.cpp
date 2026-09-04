#include "ArenaObserverHUD.h"

#include "Async/Async.h"
#include "Engine/Canvas.h"
#include "Engine/Engine.h"

AArenaObserverHUD::AArenaObserverHUD()
{
	PrimaryActorTick.bCanEverTick = false;
}

void AArenaObserverHUD::BeginPlay()
{
	Super::BeginPlay();

	const TWeakObjectPtr<AArenaObserverHUD> WeakThis(this);
	Async(EAsyncExecution::ThreadPool, [WeakThis]()
	{
		FArenaLeaderboardResult Result = FArenaLeaderboardClient::Fetch();
		AsyncTask(ENamedThreads::GameThread, [WeakThis, Result = MoveTemp(Result)]() mutable
		{
			if (AArenaObserverHUD* Hud = WeakThis.Get())
			{
				Hud->ApplyResult(MoveTemp(Result));
			}
		});
	});
}

void AArenaObserverHUD::ApplyResult(FArenaLeaderboardResult&& Result)
{
	bConnected = Result.bSuccess;
	StatusText = Result.bSuccess
		? MoveTemp(Result.Status)
		: FString::Printf(TEXT("Leaderboard unavailable: %s"), *Result.Status);
	EntryLines.Reset(Result.Entries.Num());
	for (int32 Index = 0; Index < Result.Entries.Num(); ++Index)
	{
		const FArenaLeaderboardEntry& Entry = Result.Entries[Index];
		EntryLines.Add(FString::Printf(TEXT("%d.  %s    %d"), Index + 1, *Entry.PlayerId, Entry.Score));
	}
}

void AArenaObserverHUD::DrawHUD()
{
	Super::DrawHUD();
	if (Canvas == nullptr || GEngine == nullptr)
	{
		return;
	}

	DrawRect(FLinearColor(0.01f, 0.015f, 0.025f, 1.0f), 0.0f, 0.0f, Canvas->ClipX, Canvas->ClipY);
	DrawText(TEXT("ARENA OBSERVER"), FLinearColor::White, 72.0f, 64.0f, GEngine->GetLargeFont(), 1.4f);
	DrawText(TEXT("Read-only cross-engine leaderboard"), FLinearColor(0.55f, 0.65f, 0.8f), 74.0f, 112.0f);
	DrawText(StatusText, bConnected ? FLinearColor::Green : FLinearColor::Yellow, 74.0f, 158.0f);

	float Y = 218.0f;
	if (bConnected && EntryLines.IsEmpty())
	{
		DrawText(TEXT("No scores yet"), FLinearColor::White, 74.0f, Y);
	}

	for (const FString& EntryLine : EntryLines)
	{
		DrawText(EntryLine, FLinearColor::White, 74.0f, Y);
		Y += 36.0f;
	}

	DrawText(TEXT("Restart Play to refresh"), FLinearColor(0.45f, 0.5f, 0.6f), 74.0f, Canvas->ClipY - 54.0f);
}
