#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class ItemInformation : IDisposable
    {
        public readonly ReactiveProperty<CountableItem> item = new();

        public ItemInformation(CountableItem countableItem = null)
        {
            item.Value = countableItem;
        }

        public void Dispose()
        {
            item.Dispose();
        }
    }
}

#endif
