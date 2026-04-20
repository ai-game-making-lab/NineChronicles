#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using System;
using System.Collections.Generic;
using MarketService.Response;
using Nekoyume.UI.Model;
using UniRx;

namespace Nekoyume.UI.Module
{
    public interface IShopView
    {
        public void Show(
            ReactiveProperty<List<ItemProductResponseModel>> itemProducts,
            ReactiveProperty<List<FungibleAssetValueProductResponseModel>> fungibleAssetProducts,
            Action<ShopItem> clickItem);
    }
}

#endif
