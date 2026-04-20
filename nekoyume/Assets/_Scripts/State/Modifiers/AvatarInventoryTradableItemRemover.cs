#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Model.State;
using UnityEngine;

namespace Nekoyume.State.Modifiers
{
    [Serializable]
    public class AvatarInventoryTradableItemRemover : AvatarStateModifier
    {
        [Serializable]
        private class InnerModel
        {
            [SerializeField]
            private string tradableId;

            [SerializeField]
            private long requiredBlockIndex;

            [SerializeField]
            private int count;

            public Guid TradableId => Guid.Parse(tradableId);
            public long RequiredBlockIndex => requiredBlockIndex;

            public int Count
            {
                get => count;
                set => count = value;
            }

            public InnerModel()
            {
            }

            public InnerModel(Guid tradableId, long requiredBlockIndex, int count)
            {
                this.tradableId = tradableId.ToString();
                this.requiredBlockIndex = requiredBlockIndex;
                this.count = count;
            }

            public InnerModel(InnerModel model)
                : this(model.TradableId, model.RequiredBlockIndex, model.Count)
            {
            }
        }

        [SerializeField]
        private List<InnerModel> items = new();

        public override bool IsEmpty => items.Count == 0;

        public AvatarInventoryTradableItemRemover()
        {
        }

        public AvatarInventoryTradableItemRemover(Guid tradableId, long requiredBlockIndex, int count)
        {
            items.Add(new InnerModel(tradableId, requiredBlockIndex, count));
        }

        public void AddItem(Guid tradableId, long requiredBlockIndex, int count)
        {
            var item = FindItem(tradableId);
            if (item is not null)
            {
                item.Count += count;
            }
            else
            {
                items.Add(new InnerModel(tradableId, requiredBlockIndex, count));
            }
        }

        public override void Add(IAccumulatableStateModifier<AvatarState> modifier)
        {
            if (!(modifier is AvatarInventoryTradableItemRemover m))
            {
                return;
            }

            foreach (var item in m.items)
            {
                var existing = FindItem(item.TradableId);
                if (existing is not null)
                {
                    existing.Count += item.Count;
                }
                else
                {
                    items.Add(new InnerModel(item));
                }
            }
        }

        public override void Remove(IAccumulatableStateModifier<AvatarState> modifier)
        {
            if (!(modifier is AvatarInventoryTradableItemRemover m))
            {
                return;
            }

            foreach (var item in m.items)
            {
                var existing = FindItem(item.TradableId);
                if (existing is null)
                {
                    continue;
                }

                existing.Count -= item.Count;
                if (existing.Count <= 0)
                {
                    items.Remove(existing);
                }
            }
        }

        public override AvatarState Modify(AvatarState state)
        {
            if (state is null)
            {
                return null;
            }

            foreach (var item in items)
            {
                state.inventory.RemoveTradableItem(
                    item.TradableId,
                    item.RequiredBlockIndex,
                    item.Count);
            }

            return state;
        }

        private InnerModel FindItem(Guid tradableId)
        {
            return items.FirstOrDefault(item => item.TradableId == tradableId);
        }
    }
}

#endif
