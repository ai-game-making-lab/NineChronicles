#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using System.Collections.Generic;
using System.Linq;
using Libplanet.Types.Assets;
using Nekoyume.Helper;
using Nekoyume.SingleClient.State;
using Nekoyume.State;
using Nekoyume.UI.Model;
using Nekoyume.UI.Scroller;
using UnityEngine;

namespace Nekoyume.UI.Module.WorldBoss
{
    public class WorldBossRuneStoneInventory : WorldBossDetailItem
    {
        [SerializeField]
        private RuneStoneInventoryScroll scroll;

        public void Show()
        {
            var items = new List<RuneStoneInventoryItem>();
            var worldBossSheet = Game.Game.instance.TableSheets.WorldBossListSheet;
            var bossIds = worldBossSheet.Values.Select(x => x.BossId).Distinct();
            foreach (var bossId in bossIds)
            {
                if (!WorldBossFrontHelper.TryGetRunes(bossId, out var runeRows))
                {
                    continue;
                }

                var runeStones = new List<FungibleAssetValue>();
                foreach (var row in runeRows)
                {
                    if (ClientStateViewProvider.Current.CurrentAvatarBalancesRaw.ContainsKey(row.Ticker))
                    {
                        runeStones.Add(ClientStateViewProvider.Current.CurrentAvatarBalancesRaw[row.Ticker]);
                    }
                }

                var item = new RuneStoneInventoryItem(runeStones, bossId);
                items.Add(item);
            }

            scroll.UpdateData(items);
        }
    }
}

#endif
