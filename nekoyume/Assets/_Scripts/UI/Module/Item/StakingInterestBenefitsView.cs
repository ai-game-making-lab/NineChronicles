using Nekoyume.Game;
using Nekoyume.Helper;
using Nekoyume.SingleClient.Models.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    public class StakingInterestBenefitsView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI countText;

        /// <summary>
        /// Snapshot-facing primary overload. Callers that still hold a lib9c <c>ItemBase</c>
        /// should project via <c>ItemSnapshotMapper.ToPolySnapshot()</c> before calling; this
        /// view no longer depends on <c>Nekoyume.Model.Item</c> types.
        /// </summary>
        public void Set(IItemSnapshot snapshot, int count)
        {
            iconImage.sprite = BaseItemView.GetItemIcon(snapshot);
            countText.text = $"+{count}";
        }

        public void Set(int runeId, int count)
        {
            iconImage.sprite = TableSheets.Instance.ItemSheet.TryGetValue(runeId, out var row)
                ? SpriteHelper.GetItemIcon(runeId, row.ItemSubType, row.Grade)
                : SpriteHelper.GetItemIcon(runeId);
            countText.text = $"+{count}";
        }

        public void Set(string ticker, int count, bool useCurrencyNotation = false)
        {
            iconImage.sprite = SpriteHelper.GetFavIcon(ticker);
            countText.text = $"+{(useCurrencyNotation ? count.ToCurrencyNotation() : count)}";
        }
    }
}
