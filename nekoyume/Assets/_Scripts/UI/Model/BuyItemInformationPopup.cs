#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class BuyItemInformationPopup : IDisposable
    {
        public readonly Subject<BuyItemInformationPopup> OnClickSubmit = new();
        public readonly ReactiveProperty<ItemInformation> itemInformation = new();
        public bool isSuccess;
        public ICollection<CombinationMaterial> materialItems;

        public BuyItemInformationPopup(CountableItem countableItem = null)
        {
            itemInformation.Value = new ItemInformation(countableItem);
        }

        public void Dispose()
        {
            OnClickSubmit.Dispose();
            itemInformation.Dispose();
        }
    }
}

#endif
