#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Collections.Generic;
using System.Globalization;
using Nekoyume.Helper;
using Nekoyume.Model.Item;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.UI.Module;
using UnityEngine;
using ItemType = Nekoyume.Model.Item.ItemType;

namespace Nekoyume
{
    [RequireComponent(typeof(BaseItemView))]
    public class MailRewardItemView : MonoBehaviour
    {
        [SerializeField]
        private BaseItemView baseItemView;

        [SerializeField]
        private GameObject effect;

        private readonly List<IDisposable> _disposables = new();

        public void Set(MailReward mailReward)
        {
            _disposables.DisposeAllAndClear();
            baseItemView.ClearItem();
            baseItemView.RuneSelectMove.SetActive(false);
            baseItemView.SelectCollectionObject.SetActive(false);
            baseItemView.SelectArrowObject.SetActive(false);

            if (mailReward.ItemBase is not null)
            {
                baseItemView.ItemImage.overrideSprite =
                    BaseItemView.GetItemIcon(mailReward.ItemBase);

                var data = baseItemView.GetItemViewData(mailReward.ItemBase);
                baseItemView.GradeImage.overrideSprite = data.GradeBackground;
                baseItemView.GradeHsv.range = data.GradeHsvRange;
                baseItemView.GradeHsv.hue = data.GradeHsvHue;
                baseItemView.GradeHsv.saturation = data.GradeHsvSaturation;
                baseItemView.GradeHsv.value = data.GradeHsvValue;

                if (mailReward.ItemBase.ToPolySnapshot() is EquipmentSnapshot eqSnap && eqSnap.Level > 0)
                {
                    baseItemView.EnhancementText.gameObject.SetActive(true);
                    baseItemView.EnhancementText.text = $"+{eqSnap.Level}";
                    if (eqSnap.Level >= Util.VisibleEnhancementEffectLevel)
                    {
                        baseItemView.EnhancementImage.material = data.EnhancementMaterial;
                        baseItemView.EnhancementImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        baseItemView.EnhancementImage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    baseItemView.EnhancementText.gameObject.SetActive(false);
                    baseItemView.EnhancementImage.gameObject.SetActive(false);
                }

                baseItemView.OptionTag.gameObject.SetActive(true);
                baseItemView.OptionTag.Set(mailReward.ItemBase);

                baseItemView.CountText.gameObject.SetActive(
                    mailReward.ItemBase.ItemType == ItemType.Material);
                baseItemView.CountText.text = mailReward.Count.ToString();
            }
            else
            {
                var fav = mailReward.FavFungibleAssetValue;
                var grade = Util.GetTickerGrade(fav.Currency.Ticker);
                baseItemView.ItemImage.overrideSprite = fav.GetIconSprite();

                var data = baseItemView.GetItemViewData(grade);
                baseItemView.GradeImage.overrideSprite = data.GradeBackground;
                baseItemView.GradeHsv.range = data.GradeHsvRange;
                baseItemView.GradeHsv.hue = data.GradeHsvHue;
                baseItemView.GradeHsv.saturation = data.GradeHsvSaturation;
                baseItemView.GradeHsv.value = data.GradeHsvValue;

                baseItemView.EnhancementText.gameObject.SetActive(false);
                baseItemView.EnhancementImage.gameObject.SetActive(false);
                baseItemView.OptionTag.gameObject.SetActive(false);
                if (fav.Currency.Ticker == "NCG")
                {
                    baseItemView.CountText.text = mailReward.FavFungibleAssetValue.GetQuantityString();
                }
                else
                {
                    baseItemView.CountText.text = mailReward.FavFungibleAssetValue.ToCurrencyNotation();
                }
            }

            effect.SetActive(false);
            
            if (mailReward.ItemBase.ToPolySnapshot() is EquipmentSnapshot eqCraftSnap)
            {
                baseItemView.CustomCraftArea.SetActive(eqCraftSnap.ByCustomCraft);
            }
        }

        public void ShowEffect()
        {
            effect.SetActive(true);
        }
    }
}

#endif
