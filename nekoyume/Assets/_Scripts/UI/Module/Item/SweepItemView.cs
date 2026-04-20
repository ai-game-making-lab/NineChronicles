using System;
using System.Collections.Generic;
using Nekoyume.SingleClient.Models.Items;
using UnityEngine;

namespace Nekoyume.UI.Module
{
    using UniRx;

    [RequireComponent(typeof(BaseItemView))]
    public class SweepItemView : MonoBehaviour
    {
        [SerializeField]
        private BaseItemView baseItemView;

        private readonly List<IDisposable> _disposables = new();

        /// <summary>
        /// Snapshot-facing primary overload. Renders icon, grade panel, and the custom-craft
        /// decoration using <see cref="IItemSnapshot"/> + <see cref="EquipmentSnapshot"/> fields
        /// only — callers that still hold a lib9c <c>ItemBase</c> should project via
        /// <c>ItemSnapshotMapper.ToPolySnapshot()</c> before calling.
        /// </summary>
        public void Set(IItemSnapshot snapshot, int count)
        {
            if (snapshot is null)
            {
                return;
            }

            _disposables.DisposeAllAndClear();

            baseItemView.ClearItem();
            baseItemView.TouchHandler.gameObject.SetActive(false);
            baseItemView.MinusObject.gameObject.SetActive(false);

            var data = baseItemView.GetItemViewData(snapshot);
            baseItemView.GradeImage.overrideSprite = data.GradeBackground;
            baseItemView.GradeHsv.range = data.GradeHsvRange;
            baseItemView.GradeHsv.hue = data.GradeHsvHue;
            baseItemView.GradeHsv.saturation = data.GradeHsvSaturation;
            baseItemView.GradeHsv.value = data.GradeHsvValue;

            baseItemView.ItemImage.overrideSprite = BaseItemView.GetItemIcon(snapshot);
            baseItemView.SpineItemImage.gameObject.SetActive(false);
            baseItemView.EnhancementImage.gameObject.SetActive(false);
            baseItemView.EnhancementText.gameObject.SetActive(false);
            baseItemView.CountText.text = count.ToString();
            baseItemView.PriceText.gameObject.SetActive(false);
            baseItemView.OptionTag.gameObject.SetActive(false);
            baseItemView.RuneSelectMove.SetActive(false);
            baseItemView.SelectCollectionObject.SetActive(false);
            baseItemView.SelectArrowObject.SetActive(false);

            if (snapshot is EquipmentSnapshot equipmentSnapshot)
            {
                baseItemView.CustomCraftArea.SetActive(equipmentSnapshot.ByCustomCraft);
            }
        }
    }
}
