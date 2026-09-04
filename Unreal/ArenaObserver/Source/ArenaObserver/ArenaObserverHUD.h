#pragma once

#include "ArenaLeaderboardClient.h"
#include "GameFramework/HUD.h"

#include "ArenaObserverHUD.generated.h"

UCLASS()
class ARENAOBSERVER_API AArenaObserverHUD : public AHUD
{
	GENERATED_BODY()

public:
	AArenaObserverHUD();
	virtual void DrawHUD() override;

protected:
	virtual void BeginPlay() override;

private:
	void ApplyResult(FArenaLeaderboardResult&& Result);

	bool bConnected = false;
	FString StatusText = TEXT("Connecting to 127.0.0.1:7777...");
	TArray<FString> EntryLines;
};
