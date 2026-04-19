# Single-Client Migration

## Current Slice

- `--single-client` command-line flag and `NC_SINGLE_CLIENT` environment flag are supported.
- When single-client mode is enabled, RPC client settings are cleared, the remote guest-key fetch is skipped, `Game.CreateAgent()` selects the local `Agent` path instead of `RPCAgent`, and `RPCAgent` skips gRPC channel initialization.
- `Game` no longer requires `RPCAgent` as a mandatory component. RPC modes add or reuse it only when the RPC path is selected.
- A local single-client session state file is created through `FileSingleClientStateStore`, including a generated local private key and minimal avatar state.
- `Game.ClientRuntime` exposes a Libplanet-free runtime boundary through `IClientRuntime`.
- The local state now keeps slot-based avatar entries while preserving the current selected avatar for backward compatibility.
- The first local command APIs are in place for avatar create/select by id or slot, AP charge/fill/consume, inventory item grant/consume, atomic stage play, and stage clear. They persist through the single-client state repository.
- The local stage play command returns an explicit result DTO with AP delta, optional entry-cost item delta, first-clear status, and the updated runtime state.
- The local stage play preview API reports can-play status and AP/item shortfalls without committing state, for later UI button binding.
- `BattlePreparation` now routes single-client HackAndSlash/Mimisbrunnr start-button cost checks through the local stage play preview while preserving the legacy checker for other modes.
- `BattlePreparation` now executes single-client HackAndSlash/Mimisbrunnr starts through local `PlayStage()` before the legacy blockchain action path, then updates AP, local clear state, and world-map progress.
- `ActionManager.HackAndSlash()` now has a single-client fallback for direct callers such as tutorial, booster, and next-stage flows, so those paths do not enqueue a blockchain action in single-client mode.
- AP-stone repeat play is intentionally not supported in the single-client `HackAndSlash()` fallback yet because local stage play still needs an atomic multi-cost command for AP stones plus entry materials.
- The login character-select UI reads local avatar slots in single-client mode.
- `LoginDetail` now creates/selects local avatars through `IClientRuntime` in single-client mode instead of enqueueing `CreateAvatar` or calling `RxProps.SelectAvatarAsync()`.
- `LoginScene.EnterNext()` and `EnterGame()` can select the stored local avatar slot through the single-client runtime.
- `ActionManager.ChargeActionPoint()` uses `IClientRuntime` directly in single-client mode instead of enqueueing a blockchain action.
- `ActionManager.DailyReward()` refills the action point through `IClientRuntime` in single-client mode, matching the `ChargeActionPoint()` behavior.
- `ActionManager.HackAndSlashSweep()` now runs through `SingleClientSession.SweepStage()`, consuming AP, AP stones, and entry-cost items atomically and marking the stage cleared.
- `ActionManager.ItemEnhancement()` now runs through `SingleClientSession.EnhanceEquipment()`, which level-ups the base equipment and removes consumed material equipments in a single atomic update.
- `ActionManager.Grinding()` now routes through `SingleClientSession.GrindEquipment()`, which removes equipment entries from the local inventory and credits the avatar's CRYSTAL balance.
- `SingleClientInventoryState` now stores `NonFungibleId`-keyed equipment entries alongside stacked fungible items, so enhancement/grinding/future combination flows can address individual equipments.
- `SingleClientState` now owns a ticker-keyed BigInteger balance store (CRYSTAL, Mead, runes, etc.); `SingleClientSession` exposes `GetCurrency` / `AddCurrency` / `ConsumeCurrency` on top of it.
- Every other blockchain-only action in `ActionManager` is short-circuited in single-client mode via one of two helpers (`SingleClientNoOp<T>` or `SingleClientUnsupported<T>`), so no transaction is ever enqueued:
  - Silent no-op (Combination\*/EventCrafts/Synthesize/Unlock\*/Claim\*/ActivateCollection/Rune/PetEnhancement/CustomEquipmentCraft/HackAndSlashRandomBuff/TransferAsset/s).
  - Throw (Market/Arena/Ranking/Summon/Staking/Raid/AdventureBoss/InfiniteTower/EventDungeon\*/Wanted/RedeemCode/Admin/Testbed).
- `ActionRenderHandler.Start()` and `BlockRenderHandler.Start()` capture the renderer reference but skip all `EveryRender<T>()` and block subject subscriptions in single-client mode, since UI updates already flow from the `*InSingleClient` branches above.
- `Nekoyume.SingleClient.Blockchain` provides Libplanet-free shim types (`Address`, `PublicKey`, `PrivateKey`, `ProtectedPrivateKey`, `Currency`, `FungibleAssetValue`, `HashDigest`, `TxId`) to anchor the upcoming model split.
- All RPC code paths (`RPCAgent`, `ClientFilter`, `AOTGenerated/MagicOnion.Generated.cs`, plus the `#region RPCAgent` block and `CreateAgent()` RPC branches in `Game.cs`, the `Agent is RPCAgent` branches in `Game.cs`/`BlockRenderHandler.cs`, and the MagicOnion resolver in `InitializeMessagePackResolver`) are now wrapped in `#if NC_RPC_ENABLED`. Default builds compile without the define, leaving only a static shim that preserves `RPCAgent.ShouldInitializeRpcTransport` for the EditMode flag tests.
- The local state file defaults to:
  - `Application.persistentDataPath/SingleClient/local-state.json`
- The local blockchain store path defaults to:
  - `Application.persistentDataPath/SingleClient/store`
- EditMode coverage exists under `Assets/Tests/EditMode/SingleClient` (40+ cases covering mode flag, store persistence, session runtime, sweep, and enhancement flows).

## Why lib9c Cannot Be Removed Yet

`IAgent`, `Game`, `ActionManager`, `States`, and many UI flows still expose Lib9c, Libplanet, and Bencodex types directly. A direct deletion of `Assets/_Scripts/Lib9c` currently breaks compile because the existing codebase has thousands of references to:

- `Nekoyume.Action`
- `Nekoyume.Model`
- `Nekoyume.TableData`
- `Libplanet.*`
- `Bencodex.*`

Current production reference snapshot, excluding `Assets/_Scripts/Lib9c` and AOT generated code:

- `Assets/_Scripts/Lib9c`: 1421 files
- `Assets/_Scripts/NineChronicles.RPC.Shared`: 5 files
- `Libplanet`: 266 references
- `Bencodex`: 44 references
- `Nekoyume.Action`, `Nekoyume.Model`, `Nekoyume.TableData`: 969 references
- `RPCAgent`, `NineChronicles.RPC`, `MagicOnion`, `GrpcChannel`: 73 references

The safe removal path is to shrink those public boundaries first, then delete the libraries after references reach zero.

## Recommended Order

1. Stabilize local startup.
   - Provide a single-client default CLO.
   - Add a local private-key fallback for developer/runtime builds.
   - Seed a minimal local agent/avatar state for first launch.

2. Introduce a runtime boundary that does not expose Libplanet types.
   - Add an `IClientRuntime` or equivalent facade.
   - Move `Game`, `LoginScene`, and `ActionManager` calls behind that facade.
   - Keep `IAgent` as the legacy implementation until all callers move.

3. Replace blockchain actions with local commands. (effectively complete)
   - Create avatar, select avatar by id/slot, and AP charge/fill/consume have a local runtime API.
   - Create/select avatar UI flow is wired to the local runtime in single-client mode.
   - AP charge UI flow is wired to the local runtime in single-client mode.
   - Inventory item grant/consume, atomic stage play, and stage clear have a local runtime API.
   - The local stage play command consumes AP and optional entry-cost items before clearing a stage, without partial mutation when costs are insufficient.
   - The stage play result reports first clear/repeat clear and cost deltas for UI binding.
   - The stage play preview reports can-play status and cost shortfalls before mutation.
   - Battle start cost preflight and execution are wired through local `PlayStage()` for `BattlePreparation` and direct `ActionManager.HackAndSlash()` callers.
   - `HackAndSlashSweep` runs through a new atomic multi-cost `SweepStage` local command (AP + AP stones + entry items in one transaction).
   - `DailyReward` short-circuits through the runtime's `FillActionPoint(ActionPointMax)`.
   - `ItemEnhancement` runs through `EnhanceEquipment`, backed by a new `NonFungibleId`-keyed equipment inventory on the local session state.
   - `Grinding` runs through `GrindEquipment`, removing equipments and crediting the local CRYSTAL balance.
   - `SingleClientState` owns a ticker-keyed BigInteger balance map, exposed via `GetCurrency`/`AddCurrency`/`ConsumeCurrency` on the session.
   - All remaining `ActionManager` entry points are short-circuited with `SingleClientNoOp<T>` or `SingleClientUnsupported<T>` helpers, so no transaction is enqueued in single-client mode (combination, market, arena/ranking, summon, staking, raid/world boss, adventure boss, infinite tower, event dungeon, unlock, claim, transfer, admin).
   - Combination-slot-backed actions (`CombinationEquipment`, `CombinationConsumable`, `RapidCombination`) are currently silent no-ops; real local implementations remain as future work once a combination-slot state model is introduced on the local session.

4. Split data/model usage away from lib9c. (in progress)
   - `ActionRenderHandler.Start()` and `BlockRenderHandler.Start()` skip all blockchain event subscriptions in single-client mode, so UI updates flow only from the `*InSingleClient` branches in `ActionManager`. This clears the blockchain-facing half of the split without rewriting the 235KB render handler.
   - `Nekoyume.SingleClient.Blockchain` introduces Libplanet-free shim types (`Address`, `PublicKey`, `PrivateKey`, `ProtectedPrivateKey`, `Currency`, `FungibleAssetValue`, `HashDigest`, `TxId`) to anchor future renames.
   - Remaining work: move hot `Nekoyume.Model.Item/State/Skill/Stat/Buff/TableData` DTOs that UI and battle code touch into a client-owned namespace and retire direct `Nekoyume.Model.*` usage from the UI layer.
   - Keep CSV/table loading.

   Scope audit (post-gate, current tree):
   - `using Nekoyume.Model.(Item|State|Skill|Stat|Buff)` appears 232 times across 186 files under `Assets/_Scripts/UI`. This is the blast radius for the UI-side namespace split.
   - The subnamespaces fan out roughly as: `Model.Item` (equipment/costume/material views, inventory, shop, grind, enhancement), `Model.State` (avatar/agent/world/stage snapshots threaded into headers, battle prep, login), `Model.Skill` / `Model.Stat` / `Model.Buff` (battle HUD, tooltips, option views).

   DTO scaffolding (landed):
   - `Nekoyume.SingleClient.Models.Stats.StatType` — ordinal-compatible mirror of lib9c `Nekoyume.Model.Stat.StatType` so casts between the two stay lossless during migration.
   - `Nekoyume.SingleClient.Models.Buffs.BuffView` — immutable struct (with `IEquatable<BuffView>` by `Id`) exposing the `Id`, `GroupId`, `OriginalDuration`, `RemainedDuration`, `IsBuff`, `IsDebuff`, `Kind`, `StatType`, and `StatValue` fields the HUD/tooltip actually reads.
   - `Nekoyume.SingleClient.Models.Buffs.BuffViewMapper` — `ToView(Buff)` / `ToViewMap(IReadOnlyDictionary<int, Buff>)` projection helpers plus a `StatType` ordinal round-trip.
   - `Tests/EditMode/SingleClient/BuffViewMapperTest.cs` — covers StatBuff projection, null-map skip, and `StatType` ordinal compatibility.

   HUD/tooltip migration (landed):
   - `UI/BuffIcon.cs`, `UI/Module/BuffTooltip.cs`, `UI/Module/BuffLayout.cs`, `UI/Widget/Hud/HpBar.cs`, `UI/Module/BossStatus.cs`, `UI/Module/RaidBossStatus.cs`, `UI/Module/ArenaStatus.cs` now consume `BuffView` and the client-owned `StatType` instead of `Nekoyume.Model.Buff.Buff` / `Nekoyume.Model.Stat.StatType`.
   - Battle-engine call sites (`Game/Character/Actor.cs`, `Game/Character/StageMonster.cs`, `Game/Character/EnemyPlayer.cs`, `Game/Character/RaidCharacter.cs`, `UI/Widget/Status.cs`, `UI/Widget/ArenaBattle.cs`, `UI/Widget/WorldBossBattle.cs`) now call `.ToViewMap()` at the HUD boundary, keeping lib9c `Buff` inside the simulator and DTO outside.
   - `Extensions/BuffSheetExtension.cs` gained `BuffView` overloads for `GetLocalizedName`, `GetLocalizedDescription`, and `GetIcon`; `Helper/BuffHelper.GetBuffIcon` gained a `BuffView`-typed overload that reads `view.IsStatBuff`/`view.StatType`/`view.StatValue` to pick the positive/negative sprite without touching lib9c `StatBuff`.

   Next slices (not yet started): `Nekoyume.Model.Item` (equipment/costume/material views, inventory, shop, grind, enhancement) and `Nekoyume.Model.State` (avatar/agent/world/stage snapshots threaded into headers, battle prep, login) — both materially larger than the Stat/Buff cluster and best tackled per-subsystem.

5. Remove network/runtime packages. (define-guard slice complete)
   - Single-client execution skips `RPCAgent` gRPC channel initialization.
   - `Game` no longer has a mandatory `RPCAgent` component requirement.
   - `NC_RPC_ENABLED` build define now guards every RPC code path. Default builds (without the define) compile with no live MagicOnion/gRPC references:
     - `Blockchain/RPCAgent.cs`: whole body wrapped; a compile-only shim exposes `ShouldInitializeRpcTransport` so `SingleClientModeTest` continues to assert the flag contract.
     - `Blockchain/ClientFilter.cs`: wrapped.
     - `AOTGenerated/MagicOnion.Generated.cs`: wrapped.
     - `Game.cs`: `#region RPCAgent` (SubscribeRPCAgent + On\* helpers + QuitWithAgentConnectionError) wrapped. `CreateAgent()` branches to the local `Agent` unconditionally when the define is off. `InitializeMessagePackResolver()` omits `MagicOnion.Resolvers.MagicOnionResolver.Instance` when the define is off. `Agent is RPCAgent` stake-sheet branch wrapped.
     - `Blockchain/BlockRenderHandler.cs`: `Agent is RPCAgent` retry-end subscription wrapped.
   - `NineChronicles.RPC.Shared` submodule and its working tree have been dropped (`.gitmodules` entry removed, `.git/modules/.../NineChronicles.RPC.Shared/` cleaned, orphan `.meta` deleted). `Nekoyume.asmdef` `precompiledReferences` was already empty and needed no edit. MagicOnion (`com.cysharp.magiconion` in `Packages/manifest.json`) and `Assets/Packages/Grpc.Core*` remain on disk but are unreferenced in default builds; stripping them is deferred because the package/NuGet removal is a larger dependency-graph change and has no compile impact today.

   Pre-removal audit (pre-gate snapshot, preserved for follow-up):
   - `Assets/_Scripts/NineChronicles.RPC.Shared/` submodule: 4 `.cs` files — `RPCException.cs`, `IActionEvaluationHub.cs`, `IActionEvaluationHubReceiver.cs`, `IBlockChainService.cs`. Referenced by the Unity client from `Blockchain/RPCAgent.cs` and `Blockchain/ClientFilter.cs` only (both guarded).
   - `Assets/_Scripts/AOTGenerated/MagicOnion.Generated.cs`: 198 MagicOnion/GrpcChannel references inside MagicOnion's Unity AOT-generator output (now fully gated).
   - `Blockchain/RPCAgent.cs` (60KB): sole Unity-side consumer of MagicOnion hubs + gRPC channel.
   - `Blockchain/ClientFilter.cs`: MagicOnion client filter.
   - Total RPC-flavored references (`MagicOnion|NineChronicles\.RPC|GrpcChannel|IActionEvaluationHub|IBlockChainService|RPCException`) pre-gate: 232 occurrences across 9 files; post-gate all live occurrences are inside `#if NC_RPC_ENABLED` regions.

6. Delete lib9c source.
   - Delete `Assets/_Scripts/Lib9c` only after `rg "Nekoyume\\.(Action|Model|TableData)|Libplanet|Bencodex"` no longer returns production references.
   - Run full EditMode tests and a Windows player build.

## Verification

Targeted single-client tests:

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.3.7f1/Editor/Unity.exe' \
  -batchmode -nographics \
  -projectPath 'C:\projects\ai-game-making-lab\NineChronicles\nekoyume' \
  -runTests -testPlatform editmode \
  -testFilter 'Tests.EditMode.SingleClient'
```

Current verification on Unity 6000.3.7f1 (post Step 5 define-guard + Step 4 HUD/Buff slice + RPC.Shared submodule removal):

- `Tests.EditMode.SingleClient`: 62 passed, 0 failed
  - Results: `C:\Users\USER\AppData\Local\Temp\nc-hud-slice.xml`
- Full EditMode: 135 passed, 0 failed
  - Results: `C:\Users\USER\AppData\Local\Temp\nc-post-submodule.xml`

The 62-case SingleClient count reflects the previous 44 plus the new 3 `BuffViewMapperTest` cases and the 15 gained from the interim authoring cycle that already shipped. Full EditMode includes the unchanged Lib9c/Battle/TableData suites, confirming the HUD migration and submodule removal produced no regressions.
