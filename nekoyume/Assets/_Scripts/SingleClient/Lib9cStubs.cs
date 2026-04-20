// ============================================================================
// Lib9cStubs.cs — Post-Lib9c-removal compile stubs
// ============================================================================
// This file exists solely to satisfy C# type resolution after the lib9c
// submodule was deleted (commit 00000000 "WIP(lib9c-removal): delete lib9c
// submodule"). Each namespace / class / enum stub below corresponds to a
// specific compile error class encountered in the post-deletion Unity batchmode
// log (_workspace/ninechronicles/s9d_post_delete_compile1.log).
//
// Stubs are intentionally minimal:
//   - Empty class bodies (unless specific fields/methods are read-only needed)
//   - Enums are placeholders with a single NONE value
//   - No behavior — all method stubs return default
//
// The compile target is: Nekoyume.asmdef assembly still resolves, UI/ code
// continues to TYPE-check. Runtime behavior is *broken* for any lib9c-dependent
// path (the project's SingleClient layer handles the real runtime surface).
//
// Follow-up sessions will progressively tighten these stubs (add missing
// methods/fields revealed by the next compile pass, reduce error count toward
// zero) or delete stubs as their consumers get migrated to SingleClient DTOs.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Numerics;

// ============================================================================
// Lib9c.Renderers
// ============================================================================
namespace Lib9c.Renderers
{
    /// <summary>Mirror of lib9c ActionEvaluation generic with minimal surface.</summary>
    public class ActionEvaluation<T> where T : class
    {
        public T Action;
        public Libplanet.Crypto.Address Signer;
        public long BlockIndex;
        public Exception Exception;
        public System.Collections.Generic.IReadOnlyList<string> RandomSeed;
    }

    public class BlockRenderer
    {
        public UniRx.Subject<(Libplanet.Types.Blocks.Block OldTip, Libplanet.Types.Blocks.Block NewTip)> BlockSubject
            = new UniRx.Subject<(Libplanet.Types.Blocks.Block, Libplanet.Types.Blocks.Block)>();
    }

    public class ActionRenderer
    {
        public UniRx.Subject<object> BlockEndSubject = new UniRx.Subject<object>();
        public UniRx.Subject<object> ActionRenderSubject = new UniRx.Subject<object>();
        public IObservable<ActionEvaluation<T>> EveryRender<T>() where T : class => UniRx.Observable.Empty<ActionEvaluation<T>>();
        public IObservable<ActionEvaluation<T>> EveryUnrender<T>() where T : class => UniRx.Observable.Empty<ActionEvaluation<T>>();
    }
}

// ============================================================================
// Nekoyume.Action — 60+ action types, all empty ActionBase subclasses
// ============================================================================
namespace Nekoyume.Action
{
    public class ActionBase
    {
        public Guid Id { get; set; }
        public virtual Bencodex.Types.IValue PlainValue { get; }
    }

    public class GameAction : ActionBase { }

    // Core action types — matching names from ActionManager.cs signatures.
    public class CreateAvatar : GameAction { public int index; public int hair; public int lens; public int ear; public int tail; public string name; }
    public class HackAndSlash : GameAction { }
    public class HackAndSlashSweep : GameAction { }
    public class HackAndSlashRandomBuff : GameAction { }
    public class EventDungeonBattle : GameAction { }
    public class EventDungeonBattleSweep : GameAction { }
    public class CombinationConsumable : GameAction { }
    public class CombinationEquipment : GameAction { }
    public class EventConsumableItemCrafts : GameAction { }
    public class EventMaterialItemCrafts : GameAction { }
    public class Buy : GameAction { }
    public class BuyProduct : GameAction { }
    public class RegisterProduct : GameAction { }
    public class CancelProductRegistration : GameAction { }
    public class ReRegisterProduct : GameAction { }
    public class DailyReward : GameAction { }
    public class ItemEnhancement : GameAction
    {
        public static IEnumerable<int> HammerIds => Array.Empty<int>();
        public static int GetEquipmentMaxLevel(object equipment, object costSheet) => 0;
    }
    public class ItemEnhancement13 : GameAction
    {
        public class ResultModel
        {
            public EnhancementResult enhancementResult;
            public object preItemUsable;
            public object itemUsable;
            public Libplanet.Types.Assets.FungibleAssetValue CRYSTAL;
        }
        public enum EnhancementResult { Success, GreatSuccess, Fail }
    }
    public class RankingBattle : GameAction { }
    public class BattleArena : GameAction { }
    public class PatchTableSheet : GameAction { }
    public class RapidCombination : GameAction { }
    public class RedeemCode : GameAction { }
    public class ChargeActionPoint : GameAction { }
    public class Grinding : GameAction { }
    public class Synthesize : GameAction { }
    public class UnlockEquipmentRecipe : GameAction { }
    public class UnlockWorld : GameAction { }
    public class Raid : GameAction { }
    public class ClaimRaidReward : GameAction { }
    public class ClaimWorldBossReward : GameAction { }
    public class RuneEnhancement : GameAction { }
    public class UnlockRuneSlot : GameAction { }
    public class UnlockCombinationSlot : GameAction { }
    public class PetEnhancement : GameAction { }
    public class ActivateCollection : GameAction { }
    public class ApprovePledge : GameAction { }
    public class RequestPledge : GameAction { }
    public class AuraSummon : GameAction { }
    public class RuneSummon : GameAction { }
    public class CostumeSummon : GameAction { }
    public class ClaimItems : GameAction { }
    public class TransferAsset : GameAction { }
    public class TransferAssets : GameAction { }
    public class Stake : GameAction { }
    public class ClaimStakeReward : GameAction { }
    public class ClaimUnbonded : GameAction { }
    public class ClaimReward : GameAction { }
    public class ClaimGifts : GameAction { }
    public class ClaimPatrolReward : GameAction { }
    public class CreateTestbed : GameAction { }
    public class CreateArenaDummy : GameAction { }
    public class ManipulateState : GameAction { }
    public class InfiniteTowerBattle : GameAction { }
    public class RewardGold : GameAction { }
    public class GameConfig : GameAction { }

    // AdventureBoss actions
    public class Wanted : GameAction { }
    public class ExploreAdventureBoss : GameAction { }
    public class SweepAdventureBoss : GameAction { }
    public class UnlockFloor : GameAction { }
    public class ClaimAdventureBossReward : GameAction { }
    public class CustomEquipmentCraft : GameAction { }

    // Supporting types
    public class NCActionLoader { }
    public class MonsterCollect : GameAction { }
    public class ClaimMonsterCollectionReward : GameAction { }

    public class PurchaseInfo { }
    public class RuneSlotInfo { }
}

// ============================================================================
// Nekoyume.Action.AdventureBoss, Guild, ValidatorDelegation, CustomEquipmentCraft
// ============================================================================
// Sub-namespaces intentionally empty — consumers use type aliasing when needed
namespace Nekoyume.Action.AdventureBoss { public class AdventureBossPlaceholder { } }
namespace Nekoyume.Action.Guild { public class GuildPlaceholder { } }
namespace Nekoyume.Action.ValidatorDelegation { public class ValidatorPlaceholder { } }
namespace Nekoyume.Action.Arena { public class BattleArena : global::Nekoyume.Action.BattleArena { } public class JoinArena { } }
namespace Nekoyume.Action.Garages { public class LoadIntoMyGarages { } public class UnloadFromMyGarages { } public class DeliverToOthersGarages { } }

// ============================================================================
// Nekoyume.Model.Item
// ============================================================================
namespace Nekoyume.Model.Item
{
    public enum ItemType { Consumable, Costume, Equipment, Material }
    public enum LockType { None, Time, Sale }
    public enum ItemSubType
    {
        FoodA, FoodB, FoodC, FoodD,
        FullCostume, HairCostume, EarCostume, EyeCostume, TailCostume, Title,
        Weapon, Armor, Belt, Necklace, Ring,
        EquipmentMaterial, FoodMaterial, MonsterPart, NormalMaterial, Hourglass, ApStone, Chest,
        ItemSubType_Count
    }

    public abstract class ItemBase
    {
        public int Id;
        public ItemType ItemType;
        public ItemSubType ItemSubType;
        public int Grade;
        public Nekoyume.Model.Elemental.ElementalType ElementalType;
        public Guid ItemId;
        public Nekoyume.TableData.ItemSheet.Row Data;
        public virtual string GetLocalizedName(bool useColor = true, bool useElementalIcon = true) => string.Empty;
        public virtual string GetLocalizedDescription() => string.Empty;
        public virtual string GetLocalizedNonColoredName(bool useElementalIcon = true) => string.Empty;
    }

    public abstract class ItemUsable : ItemBase
    {
        public long RequiredBlockIndex;
        public Guid TradableId;
        public Guid NonFungibleId => ItemId;
        public IList<Nekoyume.Model.Skill.Skill> Skills = new List<Nekoyume.Model.Skill.Skill>();
        public IList<Nekoyume.Model.Skill.BuffSkill> BuffSkills = new List<Nekoyume.Model.Skill.BuffSkill>();
        public Nekoyume.Model.Stat.StatsMap StatsMap = new Nekoyume.Model.Stat.StatsMap();
    }

    public class Equipment : ItemUsable
    {
        public int level;
        public long Exp;
        public bool Equipped;
        public bool MadeWithMimisbrunnrRecipe;
        public Nekoyume.Model.Stat.DecimalStat Stat = new Nekoyume.Model.Stat.DecimalStat();
        public int SetId;
        public int IconId;
        public int optionCountFromCombination;
        public bool ByCustomCraft;
        public bool CraftWithRandom;
        public bool HasRandomOnlyIcon;
        public Nekoyume.Model.Stat.StatType UniqueStatType;
    }

    public class Consumable : ItemUsable { }
    public class Costume : ItemBase { public bool Equipped; }
    public class Material : ItemBase
    {
        public Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> FungibleId;
    }
    public class TradableMaterial : Material { }
    public class TradableMaterialFactory { }
    public class ItemFactory
    {
        public static ItemBase CreateItem(Nekoyume.TableData.ItemSheet.Row row, object random = null) => null;
    }
    public class ShopItem { }
    public class Inventory { }
}

// ============================================================================
// Nekoyume.Model.Elemental
// ============================================================================
namespace Nekoyume.Model.Elemental
{
    public enum ElementalType { Normal, Fire, Water, Land, Wind }
    public enum ElementalResult { Win, Lose, Draw }
}

namespace Nekoyume.EnumType { }
namespace Nekoyume.Game.Controller { }
namespace Nekoyume.L10n
{
    public static class L10nManager { public static string Localize(string key) => key; }
}

namespace Nekoyume
{
    public static class SkillExtensions
    {
        public static string GetLocalizedName(this Nekoyume.Model.Skill.SkillSheetRow row) => string.Empty;
        public static string EffectToString(int id, Nekoyume.Model.Skill.SkillType type, decimal power, int ratio, Nekoyume.Model.Stat.StatType refType) => string.Empty;
    }

    public static class SkillIconHelper
    {
        public static UnityEngine.Sprite GetSkillIcon(int skillId) => null;
    }
}

// ============================================================================
// Nekoyume.Model.Skill
// ============================================================================
namespace Nekoyume.Model.Skill
{
    public enum SkillType { Attack, Heal, Buff, Debuff }
    public enum SkillCategory { NormalAttack, BlowAttack, DoubleAttack, AreaAttack, BuffRemovalAttack, Heal, Buff, Debuff }
    public enum SkillTargetType { Enemy, Enemies, Self, Ally, AllyWithoutSelf }
    public class Skill
    {
        public int Id;
        public long Power;
        public int Chance;
        public int StatPowerRatio;
        public Nekoyume.Model.Stat.StatType ReferencedStatType;
        public SkillSheetRow SkillRow = new SkillSheetRow();
    }
    public class BuffSkill : Skill { }
    public class SkillSheetRow
    {
        public int Id;
        public SkillType SkillType;
        public SkillCategory SkillCategory;
        public SkillTargetType SkillTargetType;
        public Nekoyume.Model.Elemental.ElementalType ElementalType;
        public int HitCount;
        public int Cooldown;
    }
}

// ============================================================================
// Nekoyume.Model.Stat
// ============================================================================
namespace Nekoyume.Model.Stat
{
    public enum StatType
    {
        NONE, HP, ATK, DEF, CRI, HIT, SPD, DRV, DRR, CDMG, ArmorPenetration, Thorn
    }

    public class DecimalStat
    {
        public StatType Type;
        public decimal BaseValue;
        public decimal AdditionalValue;
        public decimal TotalValue => BaseValue + AdditionalValue;
    }

    public class StatModifier
    {
        public StatType StatType;
        public int Value;
        public enum OperationType { Add, Percentage }
        public OperationType Operation;
    }

    public class StatsMap : IEnumerable<DecimalStat>
    {
        public List<DecimalStat> Stats = new List<DecimalStat>();
        public IEnumerator<DecimalStat> GetEnumerator() => Stats.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        public long GetBaseStat(StatType t) => 0;
        public long GetStat(StatType t) => 0;
        public IEnumerable<(StatType, long)> GetStats(bool withAdditional) => Array.Empty<(StatType, long)>();
        public IEnumerable<(StatType, long)> GetAdditionalStats(bool b) => Array.Empty<(StatType, long)>();
    }

    public class StatMap { public StatType StatType; public decimal Value; }

    public class StatsCollection { }
}

// ============================================================================
// Nekoyume.Model.State
// ============================================================================
namespace Nekoyume.Model.Quest
{
    public class WorldQuest : Quest { }
    public class CombinationEquipmentQuest : Quest { }
    public class CombinationQuest : Quest { }
    public class ItemEnhancementQuest : Quest { }
    public class TradeQuest : Quest { }
    public class ItemGradeQuest : Quest { }
    public class MonsterQuest : Quest { }
    public class GeneralQuest : Quest { }
    public class ItemTypeCollectQuest : Quest { }
    public class GoldQuest : Quest { }
}

namespace Nekoyume.Model.State
{
    public class WeeklyArenaState { public int Round; public long BlockIndex; }

    public class ClaimableReward { public List<object> Items = new List<object>(); public List<object> FungibleAssetValues = new List<object>(); }

    public class AvatarState
    {
        public Libplanet.Crypto.Address address;
        public string NameWithHash;
        public int level;
        public long exp;
        public int characterId;
        public Nekoyume.Model.Quest.QuestList questList;
        public Nekoyume.Model.Mail.MailBox mailBox;
        public Item.Inventory inventory = new Item.Inventory();
        public WorldInformation worldInformation;
        public System.Collections.Generic.Dictionary<string, object> monsterMap;
        public string name;
        public int hair, lens, ear, tail;
    }

    public class AgentState
    {
        public Libplanet.Crypto.Address address;
        public IReadOnlyDictionary<int, Libplanet.Crypto.Address> avatarAddresses = new Dictionary<int, Libplanet.Crypto.Address>();
        public long MonsterCollectionRound;
    }

    public class GoldBalanceState
    {
        public Libplanet.Types.Assets.FungibleAssetValue Gold;
    }

    public class GameConfigState { }
    public class CombinationSlotState { public int Index; public long UnlockBlockIndex; public Guid? Result; }
    public class WorldBossState { public int Id; public long StartedBlockIndex; public long EndedBlockIndex; }
    public class RaiderState { public int Level; public long ClaimedBlockIndex; public long LatestBossLevel; }
    public class World { public int Id; public int StageClearedId; }
    public class WorldInformation { public bool TryGetWorld(int id, out World world) { world = new World(); return true; } public Dictionary<int, World> world = new Dictionary<int, World>(); }
    public class StakeState { public long StartedBlockIndex; public long ReceivedBlockIndex; }
    public class MonsterCollectionState { public long StartedBlockIndex; public long ReceivedBlockIndex; public int Level; }
    public class RedeemCodeState { }
    public class StakeStateV2 : StakeState { }
    public class PledgeState { }
    public class CrystalRandomSkillState { public int StageId; public int SkillId; }
    public class HammerPointState { public int HammerPoint; public int RecipeId; }
    public class AllRuneState { public Dictionary<int, RuneState> Runes = new Dictionary<int, RuneState>(); }
    public class RuneState { public int RuneId; public int Level; }
    public class RuneSlotState { public int Index; public int RuneId; public List<RuneSlot> GetRuneSlot(object battleType) => new List<RuneSlot>(); }
    public class RuneSlot { public int Index; public int? RuneId; }
    public class ItemSlotState { }
    public class AdventureBossGameData { }
    public class PetState { public int PetId; public int Level; }
    public class CollectionState : Nekoyume.Model.Collection.CollectionState { }
    public class StakeStateV2View { }
}

namespace Nekoyume.Data
{
    public class AdventureBossGameData { }
}

// ============================================================================
// Nekoyume.Model.Mail
// ============================================================================
namespace Nekoyume.Model.Mail
{
    public enum MailType { Workshop, Auction, System, Grinding, OrderSell, OrderBuy }
    public class Mail { public long blockIndex; public Guid id; public MailType MailType; }
    public class MailBox : IEnumerable<Mail>
    {
        public List<Mail> Values = new List<Mail>();
        public IEnumerator<Mail> GetEnumerator() => Values.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class AttachmentMail : Mail { }
    public class CombinationMail : AttachmentMail { }
    public class ItemEnhanceMail : AttachmentMail { public object attachment; public Guid itemUsableId; }
    public class BuyerMail : AttachmentMail { }
    public class SellerMail : AttachmentMail { }
    public class MonsterCollectionMail : AttachmentMail { }
    public class DailyRewardMail : AttachmentMail { }
    public class GrindingMail : AttachmentMail { }
    public class OrderBuyerMail : AttachmentMail { }
    public class OrderSellerMail : AttachmentMail { }
    public class OrderExpirationMail : AttachmentMail { }
    public class CancelOrderMail : AttachmentMail { }
    public class SellCancelMail : AttachmentMail { }
    public class WorldBossRewardMail : AttachmentMail { }
    public class ClaimItemsMail : AttachmentMail { }
    public class PatrolRewardMail : AttachmentMail { }
    public class ProductBuyerMail : AttachmentMail { }
    public class ProductSellerMail : AttachmentMail { }
    public class ProductCancelMail : AttachmentMail { }
    public class UnloadFromMyGaragesRecipientMail : AttachmentMail { }
    public class CustomCraftMail : AttachmentMail { }
    public class MaterialCraftMail : AttachmentMail { }
    public class AdventureBossRaffleWinnerMail : AttachmentMail { }
}

// ============================================================================
// Nekoyume.Model.Quest
// ============================================================================
namespace Nekoyume.Model.Quest
{
    public class Quest { public int Id; public bool Complete; public bool IsPaidInAction; }
    public class QuestList : IEnumerable<Quest>
    {
        public List<Quest> Values = new List<Quest>();
        public IEnumerator<Quest> GetEnumerator() => Values.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

// ============================================================================
// Nekoyume.Model.Collection
// ============================================================================
namespace Nekoyume.Model.Collection
{
    public class CollectionState { public HashSet<int> Ids = new HashSet<int>(); }
    public class Collection { public int Id; public List<object> Materials = new List<object>(); }
}

namespace Nekoyume.Model.Grade
{
    public enum Grade { Normal, Rare, Epic, Unique, Legendary }
}

// Top-level fallback for `Grade` bare reference
namespace Nekoyume
{
    public enum Grade { Normal, Rare, Epic, Unique, Legendary }

    public class ClaimableReward
    {
        public List<object> Items = new List<object>();
        public List<object> FungibleAssetValues = new List<object>();
    }
}

// ============================================================================
// Nekoyume.Model.EnumType
// ============================================================================
namespace Nekoyume.Model.EnumType
{
    public enum BattleType { Adventure, Arena, Raid, EventDungeon, InfiniteTower }
    public enum ArenaType { Season, Championship }
    public enum RuneType { Stat, Skill }
    public enum StatReferenceType { Caster, Target }
    public enum TradeType { Sell, Buy, Cancel }
    public enum RuneSlotType { Stat, Skill }
    public enum CraftType { Recipe, CustomEquipment }
    public enum RuneUsePlace { Adventure, Arena }
    public enum Grade { Normal, Rare, Epic, Unique, Legendary }
}

// ============================================================================
// Nekoyume.Battle
// ============================================================================
// ============================================================================
// Nekoyume.Model — bare namespace used by UI scripts via `using Nekoyume.Model;`
// ============================================================================
namespace Nekoyume.Model
{
    public class ArenaCharacter { public int Level; public long HP; public int CharacterId; }
    public class Player { public int Level; }
    public class EnemyPlayer : Player { }
    public class Enemy { }
    public class RaidBoss { public long HP; public int Level; }
    public class CharacterBase { public int Level; public long HP; public int CharacterId; }
    public class WorldInformation
    {
        public class World { public int Id; public int StageClearedId; }
        public bool TryGetWorld(int id, out World world) { world = new World(); return true; }
        public System.Collections.Generic.Dictionary<int, World> world = new();
    }
}

// Nekoyume.Model sub-namespaces expected by many consumers
namespace Nekoyume.Model.Buff
{
    public class Buff { public int BuffId; public int RemainedDuration; }
    public class StatBuff : Buff { }
    public class ActionBuff : Buff { }
}
namespace Nekoyume.Model.AdventureBoss { public class AdventureBossFloor { } }
namespace Nekoyume.Model.Stake { public class StakeState { public long StartedBlockIndex; public long ReceivedBlockIndex; } }
namespace Nekoyume.Model.Market { public class Product { } public class ProductList { } public class MarketState { } }
namespace Nekoyume.Model.Arena { public class ArenaInformation { public int Score; public int Win; public int Lose; } public class ArenaScore { public int Score; } }
namespace Nekoyume.Model.InfiniteTower { public class InfiniteTowerCondition { public int Id; } }
namespace Nekoyume.Model.Rune { public class RuneEquipmentValidator { } }
namespace Nekoyume.Model.State.State { }

namespace Nekoyume.Model.BattleStatus
{
    public class ArenaLog { public int Score; public Result result; public enum Result { Lose, Win } }
    public class BattleLogStub { public int id; }
}

namespace Nekoyume.Model.BattleStatus.Arena
{
    public class ArenaAttack { public long Damage; }
    public class ArenaSkill { public int SkillId; public long Power; }
    public class ArenaTick { }
    public class ArenaDead { }
    public class ArenaBuff { }
    public class ArenaHeal { }
    public class ArenaRemoveBuffs { }
    public class ArenaEventBase { }
}

namespace Nekoyume.Battle
{
    public class BattleLog
    {
        public int id;
        public int score;
        public int worldId;
        public int stageId;
        public bool IsClear;
        public List<object> events = new List<object>();
        public Result result;
        public enum Result { Lose, Win }
    }

    public class Buff { public int BuffId; public int RemainedDuration; }
    public class StatBuff : Buff { }
    public class ActionBuff : Buff { }

    public class ArenaSkill { public int SkillId; public int Power; }
    public class CharacterBase { public int Level; public long HP; public int CharacterId; public Nekoyume.Model.Stat.StatsMap Stats = new Nekoyume.Model.Stat.StatsMap(); }
    public class Player : CharacterBase { }
    public class Enemy : CharacterBase { }
    public class CPHelper
    {
        public static long GetCP(object o) => 0;
        public static long GetCP(object o, object sheet) => 0;
        public static long GetStatCP(Nekoyume.Model.Stat.StatType type, decimal value) => 0;
        public static long DecimalToLong(decimal d) => (long)d;
        public static decimal GetSkillsMultiplier(int skillCount, int buffCount = 0) => 1m;
    }
    public class Simulator { }
    public class StageSimulator : Simulator { }
    public class ArenaSimulator : Simulator { }
    public class RaidSimulator : Simulator { }
    public class EventDungeonBattleSimulator : Simulator { }
    public class HackAndSlashSweepSimulator : Simulator { }
    public class InfiniteTowerBattleSimulator : Simulator { }
}

// ============================================================================
// Nekoyume.TableData
// ============================================================================
namespace Nekoyume.TableData
{
    public abstract class SheetRow<T>
    {
        public virtual T Key => default;
    }

    public abstract class Sheet<TKey, TValue> : IEnumerable<TValue>
    {
        public Dictionary<TKey, TValue> Values = new Dictionary<TKey, TValue>();
        public List<TValue> OrderedList = new List<TValue>();
        public TValue First => default;
        public TValue Last => default;
        public bool ContainsKey(TKey key) => false;
        public bool TryGetValue(TKey key, out TValue value) { value = default; return false; }
        public TValue this[TKey key] => default;
        public IEnumerator<TValue> GetEnumerator() => OrderedList.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ItemSheet : Sheet<int, ItemSheet.Row>
    {
        public class Row
        {
            public int Id;
            public int Grade;
            public Nekoyume.Model.Item.ItemType ItemType;
            public Nekoyume.Model.Item.ItemSubType ItemSubType;
            public Nekoyume.Model.Elemental.ElementalType ElementalType;
        }
    }

    public class EquipmentItemSheet : Sheet<int, EquipmentItemSheet.Row>
    {
        public class Row : ItemSheet.Row
        {
            public int SetId;
            public Nekoyume.Model.Stat.StatType UniqueStatType;
            public decimal Stat;
            public int SpineResourceId;
            public long Exp;
        }
    }

    public class CostumeItemSheet : Sheet<int, CostumeItemSheet.Row>
    {
        public class Row : ItemSheet.Row
        {
            public int SpineResourceId;
        }
    }

    public class MaterialItemSheet : Sheet<int, MaterialItemSheet.Row>
    {
        public class Row : ItemSheet.Row
        {
            public Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> ItemId;
        }
    }

    public class ConsumableItemSheet : Sheet<int, ConsumableItemSheet.Row>
    {
        public class Row : ItemSheet.Row
        {
            public List<object> Stats = new List<object>();
        }
    }

    public class CostumeStatSheet : Sheet<int, CostumeStatSheet.Row>
    {
        public class Row { public int CostumeId; public Nekoyume.Model.Stat.StatType StatType; public decimal Stat; }
    }

    public class SkillSheet : Sheet<int, SkillSheet.Row>
    {
        public new class Row : Nekoyume.Model.Skill.SkillSheetRow { }
    }

    public class EnhancementCostSheetV3 : Sheet<int, EnhancementCostSheetV3.Row>
    {
        public class Row
        {
            public int Level;
            public int Grade;
            public long Cost;
            public Nekoyume.Model.Item.ItemSubType ItemSubType;
            public long RequiredBlockIndex;
            public long Exp;
            public int BaseStatGrowthMin;
            public int BaseStatGrowthMax;
            public int ExtraStatGrowthMin;
            public int ExtraStatGrowthMax;
            public int ExtraSkillChanceGrowthMin;
            public int ExtraSkillChanceGrowthMax;
            public int ExtraSkillDamageGrowthMin;
            public int ExtraSkillDamageGrowthMax;
        }
        public long GetHammerExp(int id) => 0;
    }

    public class EquipmentItemRecipeSheet : Sheet<int, EquipmentItemRecipeSheet.Row> { public class Row { } }
    public class EquipmentItemSubRecipeSheetV2 : Sheet<int, EquipmentItemSubRecipeSheetV2.Row> { public class Row { public List<OptionInfo> Options = new List<OptionInfo>(); } public class OptionInfo { public int Id; public long RequiredBlockIndex; } }
    public class EquipmentItemOptionSheet : Sheet<int, EquipmentItemOptionSheet.Row> { public class Row { public int Id; public Nekoyume.Model.Stat.StatType StatType; public decimal StatMin; public decimal StatMax; } }
    public class StageSheet : Sheet<int, StageSheet.Row> { public class Row { } }
    public class WorldSheet : Sheet<int, WorldSheet.Row> { public class Row { public int Id; public int StageBegin; public int StageEnd; public string Name; } }
    public class RuneListSheet : Sheet<int, RuneListSheet.Row> { public class Row { public int Id; public int Grade; public Nekoyume.Model.EnumType.RuneType RuneType; } }
    public class RuneOptionSheet : Sheet<int, RuneOptionSheet.Row> { public class Row { public int Id; public class RuneOptionInfo { public int LevelRange; } } }
    public class RuneCostSheet : Sheet<int, RuneCostSheet.Row> { public class Row { public int RuneId; } }
    public class PetSheet : Sheet<int, PetSheet.Row> { public class Row { } }
    public class PetCostSheet : Sheet<int, PetCostSheet.Row> { public class Row { } }
    public class CollectionSheet : Sheet<int, CollectionSheet.Row> { public class Row { } }
    public class CharacterSheet : Sheet<int, CharacterSheet.Row> { public class Row { } }
    public class StakeRegularRewardSheet : Sheet<int, StakeRegularRewardSheet.Row> { public class Row { } }
    public class StakeRegularFixedRewardSheet : Sheet<int, StakeRegularFixedRewardSheet.Row> { public class Row { } }
    public class ArenaSheet : Sheet<int, ArenaSheet.Row> { public class Row { } }
    public class EventScheduleSheet : Sheet<int, EventScheduleSheet.Row> { public class Row { } }
    public class EventDungeonSheet : Sheet<int, EventDungeonSheet.Row> { public class Row { } }
    public class WorldBossListSheet : Sheet<int, WorldBossListSheet.Row> { public class Row { } }
    public class WorldBossCharacterSheet : Sheet<int, WorldBossCharacterSheet.Row> { public class Row { } }
    public class MissionSheet : Sheet<int, MissionSheet.Row> { public class Row { } }
    public class GameConfigSheet : Sheet<int, GameConfigSheet.Row> { public class Row { } }
    public class InfiniteTowerFloorSheet : Sheet<int, InfiniteTowerFloorSheet.Row> { public class Row { public int Id; public int FloorId; } }
    public class InfiniteTowerMedalSheet : Sheet<int, InfiniteTowerMedalSheet.Row> { public class Row { } }
    public class SummonSheet : Sheet<int, SummonSheet.Row> { public class Row { } }
    public class CustomEquipmentCraftRecipeSheet : Sheet<int, CustomEquipmentCraftRecipeSheet.Row> { public class Row { } }
    public class CustomEquipmentCraftOptionSheet : Sheet<int, CustomEquipmentCraftOptionSheet.Row> { public class Row { } }
    public class CrystalRandomBuffSheet : Sheet<int, CrystalRandomBuffSheet.Row> { public class Row { public int Id; public int SkillId; } }
    public class CrystalStageBuffGachaSheet : Sheet<int, CrystalStageBuffGachaSheet.Row> { public class Row { } }
    public class CrystalHammerPointSheet : Sheet<int, CrystalHammerPointSheet.Row> { public class Row { } }
    public class ItemRequirementSheet : Sheet<int, ItemRequirementSheet.Row> { public class Row { } }
    public class EquipmentItemRecipeSheetV2 : Sheet<int, EquipmentItemRecipeSheetV2.Row> { public class Row { } }
    public class WorldBossContributionRewardSheet : Sheet<int, WorldBossContributionRewardSheet.Row> { public class Row { } }
    public class WorldBossKillRewardSheet : Sheet<int, WorldBossKillRewardSheet.Row> { public class Row { } }
    public class WorldBossRankingRewardSheet : Sheet<int, WorldBossRankingRewardSheet.Row> { public class Row { } }
    public class ArenaRewardSheet : Sheet<int, ArenaRewardSheet.Row> { public class Row { } }
    public class StakeActionPointCoefficientSheet : Sheet<int, StakeActionPointCoefficientSheet.Row> { public class Row { } }
    public class PetOptionSheet : Sheet<int, PetOptionSheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData.AdventureBoss
{
    public class AdventureBossWantedRewardSheet : Nekoyume.TableData.Sheet<int, AdventureBossWantedRewardSheet.Row> { public class Row { } }
    public class AdventureBossNcgRewardRatioSheet : Nekoyume.TableData.Sheet<int, AdventureBossNcgRewardRatioSheet.Row> { public class Row { } }
    public class AdventureBossFloorSheet : Nekoyume.TableData.Sheet<int, AdventureBossFloorSheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData.Event
{
    public class EventScheduleSheet : Nekoyume.TableData.Sheet<int, EventScheduleSheet.Row> { public class Row { } }
    public class EventDungeonSheet : Nekoyume.TableData.Sheet<int, EventDungeonSheet.Row> { public class Row { } }
    public class EventDungeonStageSheet : Nekoyume.TableData.Sheet<int, EventDungeonStageSheet.Row> { public class Row { } }
    public class EventMaterialItemRecipeSheet : Nekoyume.TableData.Sheet<int, EventMaterialItemRecipeSheet.Row> { public class Row { } }
    public class EventConsumableItemRecipeSheet : Nekoyume.TableData.Sheet<int, EventConsumableItemRecipeSheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData.Rune
{
    public class RuneListSheet : Nekoyume.TableData.Sheet<int, RuneListSheet.Row> { public class Row { } }
    public class RuneCostSheet : Nekoyume.TableData.Sheet<int, RuneCostSheet.Row> { public class Row { } }
    public class RuneLevelBonusSheet : Nekoyume.TableData.Sheet<int, RuneLevelBonusSheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData.Crystal
{
    public class CrystalStageBuffGachaSheet : Nekoyume.TableData.Sheet<int, CrystalStageBuffGachaSheet.Row> { public class Row { } }
    public class CrystalRandomBuffSheet : Nekoyume.TableData.Sheet<int, CrystalRandomBuffSheet.Row> { public class Row { } }
    public class CrystalMonsterCollectionMultiplierSheet : Nekoyume.TableData.Sheet<int, CrystalMonsterCollectionMultiplierSheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData.Summon
{
    public class SummonSheet : Nekoyume.TableData.Sheet<int, SummonSheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData.CustomEquipmentCraft
{
    public class CustomEquipmentCraftRecipeSheet : Nekoyume.TableData.Sheet<int, CustomEquipmentCraftRecipeSheet.Row> { public class Row { } }
    public class CustomEquipmentCraftOptionSheet : Nekoyume.TableData.Sheet<int, CustomEquipmentCraftOptionSheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData.Stake
{
    public class StakePolicySheet : Nekoyume.TableData.Sheet<int, StakePolicySheet.Row> { public class Row { } }
}

namespace Nekoyume.TableData
{
    public class BuffLimitSheet : Sheet<int, BuffLimitSheet.Row> { public class Row { } }
    public class ArenaParticipantsSheet : Sheet<int, ArenaParticipantsSheet.Row> { public class Row { } }
    public class StatBuffSheet : Sheet<int, StatBuffSheet.Row> { public class Row { } }
    public class ActionBuffSheet : Sheet<int, ActionBuffSheet.Row> { public class Row { } }
}

namespace Nekoyume.State
{
    public class ReactiveAvatarState
    {
        public static UniRx.IReadOnlyReactiveProperty<Nekoyume.Model.State.AvatarState> ObservableAvatarState => null;
    }

    public class PetStates
    {
        public bool TryGetPetState(int id, out Nekoyume.Model.State.PetState state) { state = null; return false; }
    }
}

// ============================================================================
// Nekoyume.Exceptions
// ============================================================================
// ============================================================================
// Lib9c.DevExtensions, Lib9c.Formatters — namespaces only
// ============================================================================
namespace Lib9c.DevExtensions
{
    public class DevExtensionsPlaceholder { }
}
namespace Lib9c.DevExtensions.Action
{
    public class FaucetCurrency { }
    public class FaucetRune { }
    public class CreateOrReplaceAvatar { }
}
namespace Lib9c.Formatters
{
    public class FormatterPlaceholder { }
}

namespace Nekoyume.Exceptions
{
    public class InvalidActionException : Exception { public InvalidActionException(string m) : base(m) { } }
    public class NotEnoughMaterialException : Exception { }
    public class DuplicateActionException : Exception { }
}

// ============================================================================
// Nekoyume.Arena
// ============================================================================
namespace Nekoyume.Arena
{
    public class ArenaPlayerDigest { public int Level; public long HP; }
    public class ArenaScore { public int Score; }
    public class ArenaInformation { }
}

// ============================================================================
// Nekoyume.Extensions
// ============================================================================
namespace Nekoyume.Extensions
{
    public static class SkillSheetExtensions { }
    public static class ArenaHelper { }
    public static class WorldBossStatesHelper { }
}

// ============================================================================
// Nekoyume.Module
// ============================================================================
namespace Nekoyume.Module
{
    public static class ModuleHelper { }
}

namespace Nekoyume.Module.Guild { public class GuildModule { } }

// ============================================================================
// Game.cs central dependency shims
// ============================================================================
namespace Nekoyume.IAPStore
{
    public class IAPStoreManager { }
}

namespace GeneratedApiNamespace.ArenaServiceClient
{
    public class ArenaServiceClient { }
}

namespace Nekoyume.State
{
    public class States
    {
        public static States Instance => new States();
        public Nekoyume.Model.State.AgentState AgentState;
        public Nekoyume.Model.State.AvatarState CurrentAvatarState;
        public Nekoyume.Model.State.GoldBalanceState GoldBalanceState;
        public Nekoyume.Model.State.GameConfigState GameConfigState;
        public int CurrentAvatarKey;
        public int StakingLevel;
        public Libplanet.Types.Assets.FungibleAssetValue CrystalBalance;
        public System.Collections.Generic.Dictionary<int, Nekoyume.Model.State.AvatarState> AvatarStates = new();
    }

    public class LocalLayer
    {
        public static LocalLayer Instance => new LocalLayer();
    }

    public static class LocalLayerActions
    {
        public static void Initialize(Libplanet.Crypto.Address a) { }
    }

}

namespace Nekoyume.Blockchain
{
    public interface IAgent
    {
        long BlockIndex { get; }
        Libplanet.Crypto.Address Address { get; }
        Libplanet.Types.Blocks.BlockHash BlockTipHash { get; }
        Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> BlockTipStateRootHash { get; }
    }

    public class Agent : UnityEngine.MonoBehaviour, IAgent
    {
        public static string DefaultStoragePath => string.Empty;
        public long BlockIndex => 0;
        public Libplanet.Crypto.Address Address => default;
        public Libplanet.Types.Blocks.BlockHash BlockTipHash => default;
        public Libplanet.Common.HashDigest<System.Security.Cryptography.SHA256> BlockTipStateRootHash => default;
    }

    public class ActionManager : System.IDisposable
    {
        public static ActionManager Instance => null;
        public void Dispose() { }
    }

    public class ActionHandler { }

    public static class StateGetter
    {
        public static Bencodex.Types.IValue GetState(Libplanet.Crypto.Address a) => null;
    }
}

namespace Nekoyume.Game
{
    public static class TableSheetsStub
    {
        public static Nekoyume.TableData.ItemSheet ItemSheet => new();
    }
}

namespace Nekoyume.Helper
{
    public class CommandLineOptions
    {
        public bool Empty => false;
        public bool SingleClient;
        public bool RpcClient;
        public string RpcServerHost;
        public string[] RpcServerHosts = System.Array.Empty<string>();
        public int RpcServerPort;
        public string StoragePath;
        public string PrivateKey;
    }
}
