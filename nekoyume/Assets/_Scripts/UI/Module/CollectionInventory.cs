#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Battle;
using Nekoyume.Helper;
using Nekoyume.Model.Item;
using Nekoyume.SingleClient.Models.EnumType;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.SingleClient.State;
using Nekoyume.State;
using Nekoyume.UI.Model;
using Nekoyume.UI.Scroller;
using UnityEngine;
using Material = Nekoyume.Model.Item.Material;
using ItemType = Nekoyume.Model.Item.ItemType;

namespace Nekoyume.UI.Module
{
    using UniRx;

    public class CollectionInventory : MonoBehaviour
    {
        [SerializeField] private InventoryScroll scroll;
        [SerializeField] private CanvasGroup scrollCanvasGroup;

        private CollectionMaterial _requiredItem;
        private Action<InventoryItem> _onClickItem;
        private CollectionMaterial[] _requiredItems;
        private bool canSelect = true;

        private readonly List<InventoryItem> _items = new();

        public InventoryItem SelectedItem { get; private set; }

        public void SetInventory(Action<InventoryItem> onClickItem)
        {
            _onClickItem = onClickItem;

            ReactiveAvatarState.Inventory
                .Subscribe(UpdateInventory)
                .AddTo(gameObject);

            scroll.OnClick.Subscribe(SelectItem).AddTo(gameObject);
        }

        private void SetCanSelect(bool value)
        {
            canSelect = value;
            scrollCanvasGroup.interactable = value;
        }

#region NonFungibleItems (Equipment, Costume) - select one

        public void SetRequiredItem(CollectionMaterial requiredItem)
        {
            _requiredItem = requiredItem;
            if (_requiredItem == null)
            {
                return;
            }

            SetCanSelect(true);

            var models = GetModels(_requiredItem);
            scroll.UpdateData(models, true);
            SelectItem(models.FirstOrDefault());
        }

        private List<InventoryItem> GetModels(CollectionMaterial requiredItem)
        {
            if (requiredItem == null)
            {
                return new List<InventoryItem>();
            }

            // get from _items by required item's condition
            var row = requiredItem.Row;
            var items = _items.Where(item =>
                item.ItemBase.ItemType == requiredItem.ItemType &&
                item.ItemBase is INonFungibleItem nonFungibleItem &&
                row.Validate(nonFungibleItem)).ToList();
            if (!items.Any())
            {
                return items;
            }

            switch (items.First().ItemBase.ItemType)
            {
                case ItemType.Equipment:
                    items = items
                        .OrderBy(item => item.ItemBase.ToPolySnapshot() is EquipmentSnapshot colEqSnap ? colEqSnap.GetCP() : 0L).ToList();
                    UpdateEquipmentEquipped(items);
                    break;
                case ItemType.Costume:
                    UpdateCostumeEquipped(items);
                    break;
            }

            return items;
        }

        private void SelectItem(InventoryItem item)
        {
            if (!canSelect)
            {
                return;
            }

            SelectedItem?.CollectionSelected.SetValueAndForceNotify(false);
            SelectedItem = item;
            SelectedItem.CollectionSelected.SetValueAndForceNotify(true);
            _onClickItem?.Invoke(SelectedItem);
        }

        private static void UpdateEquipmentEquipped(List<InventoryItem> equipments)
        {
            var equippedEquipments = new List<Guid>();
            for (var i = 1; i < (int)BattleType.End; i++)
            {
                equippedEquipments.AddRange(ClientStateViewProvider.Current.CurrentItemSlotStatesRaw[((BattleType)i).ToLib9c()].Equipments);
            }

            foreach (var equipment in equipments)
            {
                var equipped =
                    equippedEquipments.Exists(x => x == ((Equipment)equipment.ItemBase).ItemId);
                equipment.Equipped.SetValueAndForceNotify(equipped);
            }
        }

        private static void UpdateCostumeEquipped(List<InventoryItem> costumes)
        {
            var equippedCostumes = new List<Guid>();
            for (var i = 1; i < (int)BattleType.End; i++)
            {
                equippedCostumes.AddRange(ClientStateViewProvider.Current.CurrentItemSlotStatesRaw[((BattleType)i).ToLib9c()].Costumes);
            }

            foreach (var costume in costumes)
            {
                var equipped =
                    equippedCostumes.Exists(x => x == ((Costume)costume.ItemBase).ItemId);
                costume.Equipped.SetValueAndForceNotify(equipped);
            }
        }

#endregion

#region FungibleItems (Consumable, Material) - select auto

        public void SetRequiredItems(CollectionMaterial[] requiredItems)
        {
            _requiredItems = requiredItems;
            SetCanSelect(false);

            // Get Models
            var models = _requiredItems.Select(requiredItem =>
                _items.First(item => item.ItemBase.Id == requiredItem.Row.ItemId)).ToList();

            scroll.UpdateData(models, true);

            // Select All
            foreach (var model in models)
            {
                model.CollectionSelected.SetValueAndForceNotify(true);
            }
        }

#endregion

#region Update Inventory

        private void UpdateInventory(Nekoyume.Model.Item.Inventory inventory)
        {
            _items.Clear();
            if (inventory == null)
            {
                return;
            }

            foreach (var item in inventory.Items)
            {
                if (item.Locked)
                {
                    continue;
                }

                AddItem(item.item, item.count);
            }

            if (canSelect)
            {
                SetRequiredItem(_requiredItem);
            }
            else
            {
                SetRequiredItems(_requiredItems);
            }
        }

        private void AddItem(ItemBase itemBase, int count = 1)
        {
            if (itemBase is ITradableItem tradableItem)
            {
                var blockIndex = Game.Game.instance.Agent?.BlockIndex ?? -1;
                if (tradableItem.RequiredBlockIndex > blockIndex)
                {
                    return;
                }
            }

            InventoryItem inventoryItem;
            switch (itemBase.ItemType)
            {
                case ItemType.Consumable:
                    var consumable = (Consumable)itemBase;
                    if (TryGetConsumable(consumable, out inventoryItem))
                    {
                        inventoryItem.Count.Value += count;
                    }
                    else
                    {
                        inventoryItem = new InventoryItem(itemBase, count, false, false);
                        _items.Add(inventoryItem);
                    }

                    break;
                case ItemType.Costume:
                case ItemType.Equipment:
                    inventoryItem = new InventoryItem(
                        itemBase,
                        count,
                        !Util.IsUsableItem(itemBase),
                        false);
                    _items.Add(inventoryItem);
                    break;
                case ItemType.Material:
                    var material = (Material)itemBase;
                    if (TryGetMaterial(material, out inventoryItem))
                    {
                        inventoryItem.Count.Value += count;
                    }
                    else
                    {
                        inventoryItem = new InventoryItem(itemBase, count, false, false);
                        _items.Add(inventoryItem);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool TryGetMaterial(Material material, out InventoryItem model)
        {
            model = _items.FirstOrDefault(item =>
                item.ItemBase.ToPolySnapshot() is MaterialSnapshot mSnap && mSnap.MatchesItemId(material));

            return model != null;
        }

        private bool TryGetConsumable(Consumable consumable, out InventoryItem model)
        {
            model = _items.FirstOrDefault(item => item.ItemBase.Id.Equals(consumable.Id) &&
                item.ItemBase.ItemType == ItemType.Consumable);

            return model != null;
        }

#endregion
    }
}

#endif
