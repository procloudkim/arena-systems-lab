#include "ArenaObserverGameMode.h"

#include "ArenaObserverHUD.h"

AArenaObserverGameMode::AArenaObserverGameMode()
{
	DefaultPawnClass = nullptr;
	HUDClass = AArenaObserverHUD::StaticClass();
}
