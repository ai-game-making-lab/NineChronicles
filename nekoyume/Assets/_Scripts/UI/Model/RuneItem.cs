using System.Linq;
using Libplanet.Types.Assets;
using Nekoyume.Helper;
using Nekoyume.SingleClient.State;
using Nekoyume.State;
using Nekoyume.TableData;
using Nekoyume.TableData.Rune;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class RuneItem
    {
        public RuneListSheet.Row Row { get; }
        public RuneOptionSheet.Row OptionRow { get; }
        public RuneCostSheet.Row CostRow { get; }

        public FungibleAssetValue RuneStone { get; set; }
        public int Level { get; }
        public bool IsMaxLevel { get; }
        public bool EnoughRuneStone { get; }
        public bool EnoughCrystal { get; }
        public bool EnoughNcg { get; }
        public bool HasNotification => EnoughRuneStone && EnoughCrystal && EnoughNcg;
        public int SortingOrder { get; }

        public readonly ReactiveProperty<bool> IsSelected = new();

        public RuneItem(RuneListSheet.Row row, int level)
        {
            Row = row;
            Level = level;

            var runeOptionSheet = Game.Game.instance.TableSheets.RuneOptionSheet;
            if (!runeOptionSheet.TryGetValue(row.Id, out var optionRow))
            {
                return;
            }

            OptionRow = optionRow;

            IsMaxLevel = Level == optionRow.LevelOptionMap.Count;

            var runeRow = Game.Game.instance.TableSheets.RuneSheet[row.Id];
            if (!ClientStateViewProvider.Current.CurrentAvatarBalancesRaw.ContainsKey(runeRow.Ticker))
            {
                return;
            }

            if (RuneFrontHelper.TryGetRuneData(runeRow.Ticker, out var runeData))
            {
                SortingOrder = runeData.sortingOrder;
            }

            RuneStone = ClientStateViewProvider.Current.CurrentAvatarBalancesRaw[runeRow.Ticker];

            var costSheet = Game.Game.instance.TableSheets.RuneCostSheet;
            if (!costSheet.TryGetValue(row.Id, out var costRow))
            {
                return;
            }

            CostRow = costRow;

            if (IsMaxLevel || !CostRow.TryGetCost(Level + 1, out var cost))
            {
                EnoughRuneStone = false;
                EnoughCrystal = false;
                EnoughNcg = false;

                return;
            }

            EnoughRuneStone = RuneStone.MajorUnit >= cost.RuneStoneQuantity;
            EnoughCrystal = ClientStateViewProvider.Current.CurrentAgentCrystalBalanceMajorUnit >= cost.CrystalQuantity;
            EnoughNcg = ClientStateViewProvider.Current.CurrentGoldBalanceStateRaw.Gold.MajorUnit >= cost.NcgQuantity;
        }
    }
}
