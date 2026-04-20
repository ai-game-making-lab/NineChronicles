#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using Libplanet.Types.Assets;
using Nekoyume.Helper;
using Nekoyume.Model.Item;
using Nekoyume.SingleClient.Models.Items;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class Item : IDisposable
    {
        public readonly ReactiveProperty<ItemBase> ItemBase = new();
        public readonly ReactiveProperty<FungibleAssetValue> FungibleAssetValue = new();
        public readonly ReactiveProperty<bool> GradeEnabled = new(true);
        public readonly ReactiveProperty<string> Enhancement = new();
        public readonly ReactiveProperty<bool> EnhancementEnabled = new(false);
        public readonly ReactiveProperty<bool> EnhancementEffectEnabled = new(false);
        public readonly ReactiveProperty<bool> Dimmed = new(false);
        public readonly ReactiveProperty<bool> Selected = new(false);
        public readonly ReactiveProperty<bool> ActiveSelf = new(true);

        public readonly Subject<Item> OnClick = new();
        public readonly Subject<Item> OnDoubleClick = new();

        public Item(ItemBase value)
        {
            ItemBase.Value = value;

            if (ItemBase.Value.ToPolySnapshot() is EquipmentSnapshot eqSnap &&
                eqSnap.Level > 0)
            {
                Enhancement.Value = $"+{eqSnap.Level}";
                EnhancementEnabled.Value = true;
                EnhancementEffectEnabled.Value = eqSnap.Level >= Util.VisibleEnhancementEffectLevel;
            }
            else
            {
                Enhancement.Value = string.Empty;
                EnhancementEnabled.Value = false;
                EnhancementEffectEnabled.Value = false;
            }
        }

        public Item(FungibleAssetValue value)
        {
            FungibleAssetValue.Value = value;
            EnhancementEnabled.Value = false;
            EnhancementEffectEnabled.Value = false;
        }

        public virtual void Dispose()
        {
            ItemBase.Dispose();
            GradeEnabled.Dispose();
            Enhancement.Dispose();
            EnhancementEnabled.Dispose();
            EnhancementEffectEnabled.Dispose();
            Dimmed.Dispose();
            ActiveSelf.Dispose();
            Selected.Dispose();

            OnClick.Dispose();
            OnDoubleClick.Dispose();
        }
    }
}

#endif
