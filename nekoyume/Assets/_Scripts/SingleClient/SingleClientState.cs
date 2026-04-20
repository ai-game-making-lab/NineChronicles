using System;
using System.Collections.Generic;
using System.Linq;

namespace Nekoyume.SingleClient
{
    [Serializable]
    public sealed class SingleClientState
    {
        public const int CurrentVersion = 1;
        public const string DefaultPlayerId = "local-player";
        public const string DefaultAvatarId = "local-avatar";
        public const string DefaultAvatarName = "Local Avatar";

        public int version = CurrentVersion;
        public string playerId = DefaultPlayerId;
        public string avatarId = DefaultAvatarId;
        public string privateKeyHex;
        public SingleClientAvatarState avatar = SingleClientAvatarState.CreateDefault(DefaultAvatarId);
        public List<SingleClientAvatarState> avatars = new();
        public SingleClientInventoryState inventory = new();
        public SingleClientStageState stage = new();
        public List<SingleClientCurrencyBalance> balances = new();
        public long blockIndex;
        public long updatedAtUnixSeconds;

        public static string GetAvatarId(int slotIndex)
        {
            return slotIndex == 0 ? DefaultAvatarId : $"{DefaultAvatarId}-{slotIndex}";
        }

        public static SingleClientState CreateDefault(
            DateTimeOffset now,
            string privateKeyHex = null)
        {
            return new SingleClientState
            {
                privateKeyHex = privateKeyHex,
                updatedAtUnixSeconds = now.ToUnixTimeSeconds()
            };
        }

        public bool EnsureDefaults(Func<string> privateKeyFactory)
        {
            var changed = false;

            if (version <= 0)
            {
                version = CurrentVersion;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(playerId))
            {
                playerId = DefaultPlayerId;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(avatarId))
            {
                avatarId = DefaultAvatarId;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(privateKeyHex))
            {
                privateKeyHex = privateKeyFactory();
                changed = true;
            }

            if (avatar is null)
            {
                avatar = SingleClientAvatarState.CreateDefault(avatarId, GetSlotIndex(avatarId));
                changed = true;
            }
            else
            {
                changed |= avatar.EnsureDefaults(avatarId, GetSlotIndex(avatarId));
            }

            changed |= EnsureAvatarSlots();

            if (inventory is null)
            {
                inventory = new SingleClientInventoryState();
                changed = true;
            }
            else
            {
                changed |= inventory.EnsureDefaults();
            }

            if (stage is null)
            {
                stage = new SingleClientStageState();
                changed = true;
            }
            else
            {
                changed |= stage.EnsureDefaults();
            }

            if (balances is null)
            {
                balances = new List<SingleClientCurrencyBalance>();
                changed = true;
            }
            else
            {
                for (var i = balances.Count - 1; i >= 0; i--)
                {
                    var entry = balances[i];
                    if (entry is null || string.IsNullOrWhiteSpace(entry.ticker))
                    {
                        balances.RemoveAt(i);
                        changed = true;
                        continue;
                    }

                    var trimmed = entry.ticker.Trim();
                    if (trimmed != entry.ticker)
                    {
                        entry.ticker = trimmed;
                        changed = true;
                    }

                    if (string.IsNullOrWhiteSpace(entry.rawValue))
                    {
                        entry.rawValue = "0";
                        changed = true;
                    }
                }

                var previousOrder = balances.Select(b => b.ticker).ToArray();
                balances.Sort((a, b) => string.CompareOrdinal(a.ticker, b.ticker));
                if (!previousOrder.SequenceEqual(balances.Select(b => b.ticker)))
                {
                    changed = true;
                }
            }

            return changed;
        }

        private bool EnsureAvatarSlots()
        {
            var changed = false;

            if (avatars is null)
            {
                avatars = new List<SingleClientAvatarState>();
                changed = true;
            }

            for (var i = avatars.Count - 1; i >= 0; i--)
            {
                var candidate = avatars[i];
                if (candidate is null)
                {
                    avatars.RemoveAt(i);
                    changed = true;
                    continue;
                }

                changed |= candidate.EnsureDefaults(
                    candidate.id,
                    candidate.slotIndex < 0 ? 0 : candidate.slotIndex);
            }

            var selectedSlotIndex = GetSlotIndex(avatarId);
            avatar.slotIndex = selectedSlotIndex;
            var selected = avatars.FirstOrDefault(candidate => candidate.slotIndex == selectedSlotIndex);
            if (selected is null)
            {
                avatars.Add(avatar.Clone());
                changed = true;
            }
            else
            {
                changed |= selected.CopyFrom(avatar);
            }

            for (var i = 0; i < avatars.Count; i++)
            {
                var candidate = avatars[i];
                var duplicate = avatars
                    .Skip(i + 1)
                    .FirstOrDefault(other => other.slotIndex == candidate.slotIndex);
                if (duplicate is null)
                {
                    continue;
                }

                candidate.CopyFrom(duplicate);
                avatars.Remove(duplicate);
                changed = true;
                i--;
            }

            var previousOrder = avatars.Select(candidate => candidate.slotIndex).ToArray();
            avatars.Sort((left, right) => left.slotIndex.CompareTo(right.slotIndex));
            if (!previousOrder.SequenceEqual(avatars.Select(candidate => candidate.slotIndex)))
            {
                changed = true;
            }

            return changed;
        }

        private static int GetSlotIndex(string id)
        {
            if (string.Equals(id, DefaultAvatarId, StringComparison.Ordinal))
            {
                return 0;
            }

            var prefix = $"{DefaultAvatarId}-";
            if (!string.IsNullOrWhiteSpace(id) &&
                id.StartsWith(prefix, StringComparison.Ordinal) &&
                int.TryParse(id.Substring(prefix.Length), out var slotIndex) &&
                slotIndex >= 0)
            {
                return slotIndex;
            }

            return 0;
        }
    }

    [Serializable]
    public sealed class SingleClientAvatarState
    {
        public int slotIndex;
        public string id;
        public string name;
        public int level;
        public long actionPoint;

        public static SingleClientAvatarState CreateDefault(string avatarId)
        {
            return CreateDefault(avatarId, 0);
        }

        public static SingleClientAvatarState CreateDefault(string avatarId, int slotIndex)
        {
            return new SingleClientAvatarState
            {
                slotIndex = slotIndex < 0 ? 0 : slotIndex,
                id = string.IsNullOrWhiteSpace(avatarId)
                    ? SingleClientState.DefaultAvatarId
                    : avatarId,
                name = SingleClientState.DefaultAvatarName,
                level = 1
            };
        }

        public bool EnsureDefaults(string avatarId)
        {
            return EnsureDefaults(avatarId, slotIndex);
        }

        public bool EnsureDefaults(string avatarId, int fallbackSlotIndex)
        {
            var changed = false;

            if (slotIndex < 0)
            {
                slotIndex = fallbackSlotIndex < 0 ? 0 : fallbackSlotIndex;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                id = string.IsNullOrWhiteSpace(avatarId)
                    ? SingleClientState.DefaultAvatarId
                    : avatarId;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = SingleClientState.DefaultAvatarName;
                changed = true;
            }

            if (level <= 0)
            {
                level = 1;
                changed = true;
            }

            return changed;
        }

        public SingleClientAvatarState Clone()
        {
            return new SingleClientAvatarState
            {
                slotIndex = slotIndex,
                id = id,
                name = name,
                level = level,
                actionPoint = actionPoint
            };
        }

        public bool CopyFrom(SingleClientAvatarState other)
        {
            var changed = false;

            if (slotIndex != other.slotIndex)
            {
                slotIndex = other.slotIndex;
                changed = true;
            }

            if (id != other.id)
            {
                id = other.id;
                changed = true;
            }

            if (name != other.name)
            {
                name = other.name;
                changed = true;
            }

            if (level != other.level)
            {
                level = other.level;
                changed = true;
            }

            if (actionPoint != other.actionPoint)
            {
                actionPoint = other.actionPoint;
                changed = true;
            }

            return changed;
        }
    }

    [Serializable]
    public sealed class SingleClientInventoryState
    {
        public List<SingleClientInventoryItemState> items = new();
        public List<SingleClientEquipmentItemState> equipments = new();

        public bool EnsureDefaults()
        {
            var changed = false;

            if (items is null)
            {
                items = new List<SingleClientInventoryItemState>();
                changed = true;
            }

            if (equipments is null)
            {
                equipments = new List<SingleClientEquipmentItemState>();
                changed = true;
            }

            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item is null || string.IsNullOrWhiteSpace(item.itemId) || item.count <= 0)
                {
                    items.RemoveAt(i);
                    changed = true;
                    continue;
                }

                var trimmedItemId = item.itemId.Trim();
                if (trimmedItemId != item.itemId)
                {
                    item.itemId = trimmedItemId;
                    changed = true;
                }
            }

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var duplicate = items
                    .Skip(i + 1)
                    .FirstOrDefault(other => other.itemId == item.itemId);
                if (duplicate is null)
                {
                    continue;
                }

                item.count += duplicate.count;
                items.Remove(duplicate);
                changed = true;
                i--;
            }

            var previousOrder = items.Select(item => item.itemId).ToArray();
            items.Sort((left, right) => string.CompareOrdinal(left.itemId, right.itemId));
            if (!previousOrder.SequenceEqual(items.Select(item => item.itemId)))
            {
                changed = true;
            }

            for (var i = equipments.Count - 1; i >= 0; i--)
            {
                var equipment = equipments[i];
                if (equipment is null || string.IsNullOrWhiteSpace(equipment.nonFungibleId))
                {
                    equipments.RemoveAt(i);
                    changed = true;
                    continue;
                }

                var trimmed = equipment.nonFungibleId.Trim();
                if (trimmed != equipment.nonFungibleId)
                {
                    equipment.nonFungibleId = trimmed;
                    changed = true;
                }
            }

            var previousEquipmentOrder = equipments.Select(e => e.nonFungibleId).ToArray();
            equipments.Sort((left, right) =>
                string.CompareOrdinal(left.nonFungibleId, right.nonFungibleId));
            if (!previousEquipmentOrder.SequenceEqual(equipments.Select(e => e.nonFungibleId)))
            {
                changed = true;
            }

            return changed;
        }

        public void Grant(string itemId, long count)
        {
            var item = Find(itemId);
            if (item is null)
            {
                items.Add(new SingleClientInventoryItemState
                {
                    itemId = itemId,
                    count = count
                });
                items.Sort((left, right) => string.CompareOrdinal(left.itemId, right.itemId));
                return;
            }

            item.count += count;
        }

        public long GetCount(string itemId)
        {
            return Find(itemId)?.count ?? 0L;
        }

        public bool TryConsume(string itemId, long count)
        {
            var item = Find(itemId);
            if (item is null || item.count < count)
            {
                return false;
            }

            item.count -= count;
            if (item.count == 0)
            {
                items.Remove(item);
            }

            return true;
        }

        private SingleClientInventoryItemState Find(string itemId)
        {
            return items.FirstOrDefault(item => item.itemId == itemId);
        }

        public SingleClientEquipmentItemState GrantEquipment(
            string nonFungibleId,
            int itemSheetId,
            int level = 0,
            long requiredBlockIndex = 0)
        {
            if (string.IsNullOrWhiteSpace(nonFungibleId))
            {
                throw new ArgumentException("nonFungibleId is required.", nameof(nonFungibleId));
            }

            var trimmed = nonFungibleId.Trim();
            var existing = FindEquipment(trimmed);
            if (existing is not null)
            {
                existing.itemSheetId = itemSheetId;
                existing.level = level;
                existing.requiredBlockIndex = requiredBlockIndex;
                return existing;
            }

            var entry = new SingleClientEquipmentItemState
            {
                nonFungibleId = trimmed,
                itemSheetId = itemSheetId,
                level = level,
                requiredBlockIndex = requiredBlockIndex,
            };
            equipments.Add(entry);
            equipments.Sort((left, right) =>
                string.CompareOrdinal(left.nonFungibleId, right.nonFungibleId));
            return entry;
        }

        public SingleClientEquipmentItemState FindEquipment(string nonFungibleId)
        {
            if (string.IsNullOrWhiteSpace(nonFungibleId))
            {
                return null;
            }

            var key = nonFungibleId.Trim();
            return equipments.FirstOrDefault(e => e.nonFungibleId == key);
        }

        public bool HasEquipment(string nonFungibleId)
        {
            return FindEquipment(nonFungibleId) is not null;
        }

        public bool RemoveEquipment(string nonFungibleId)
        {
            var entry = FindEquipment(nonFungibleId);
            if (entry is null)
            {
                return false;
            }

            equipments.Remove(entry);
            return true;
        }

        public void SetEquipmentEquipped(string nonFungibleId, bool equipped)
        {
            var entry = FindEquipment(nonFungibleId);
            if (entry is null)
            {
                return;
            }

            entry.equipped = equipped;
        }
    }

    [Serializable]
    public sealed class SingleClientInventoryItemState
    {
        public string itemId;
        public long count;
    }

    [Serializable]
    public sealed class SingleClientEquipmentItemState
    {
        public string nonFungibleId;
        public int itemSheetId;
        public int level;
        public bool equipped;
        public long requiredBlockIndex;
    }

    [Serializable]
    public sealed class SingleClientCurrencyBalance
    {
        public string ticker;
        public string rawValue;
    }

    [Serializable]
    public sealed class SingleClientStageState
    {
        public int highestClearedStageId;
        public List<int> clearedStageIds = new();

        public bool EnsureDefaults()
        {
            if (clearedStageIds is null)
            {
                clearedStageIds = new List<int>();
                return true;
            }

            var normalized = clearedStageIds
                .Where(stageId => stageId > 0)
                .Distinct()
                .OrderBy(stageId => stageId)
                .ToList();
            var changed = !clearedStageIds.SequenceEqual(normalized);

            clearedStageIds = normalized;
            var highest = clearedStageIds.Count == 0 ? 0 : clearedStageIds[^1];
            if (highestClearedStageId != highest)
            {
                highestClearedStageId = highest;
                changed = true;
            }

            return changed;
        }

        public void Clear(int stageId)
        {
            if (!clearedStageIds.Contains(stageId))
            {
                clearedStageIds.Add(stageId);
            }

            EnsureDefaults();
        }

        public bool IsCleared(int stageId)
        {
            return clearedStageIds?.Contains(stageId) == true;
        }
    }
}
