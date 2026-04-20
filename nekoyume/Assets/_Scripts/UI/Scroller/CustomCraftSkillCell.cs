#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using System;
using Nekoyume.SingleClient.Models.TableData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Scroller
{
    using UniRx;
    public class CustomCraftSkillCell : RectCell<CustomCraftSkillCell.Model, CustomCraftSkillScroll.ContextModel>
    {
        // Client-owned row mirrors replace the lib9c <c>SkillSheet.Row</c> /
        // <c>EquipmentItemOptionSheet.Row</c> pair so the cell model stays inside the UI
        // boundary. The populator (<c>CustomCraftInfoPopup.ShowSkillView</c>) calls
        // <c>.ToView()</c> at the sheet-access seam; downstream consumers (the click
        // handler + <c>SkillPositionTooltip</c>) now accept <see cref="SkillSheetRowView"/>
        // / <see cref="EquipmentItemOptionRowView"/> directly without re-projecting.
        public class Model
        {
            public string SkillName;
            public string SkillRatio;
            public SkillSheetRowView SkillRow;
            public EquipmentItemOptionRowView OptionRow;
        }

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private TextMeshProUGUI ratioText;

        [SerializeField]
        private Button detailButton;

        private SkillSheetRowView _skillRow;
        private EquipmentItemOptionRowView _optionRow;

        private void Awake()
        {
            detailButton.OnClickAsObservable()
                .ThrottleFirst(new TimeSpan(0, 0, 1))
                .Subscribe(OnClickDetailButton)
                .AddTo(gameObject);
        }

        public override void UpdateContent(Model itemData)
        {
            nameText.SetText(itemData.SkillName);
            ratioText.SetText(itemData.SkillRatio);
            _skillRow = itemData.SkillRow;
            _optionRow = itemData.OptionRow;
        }

        private void OnClickDetailButton(Unit _)
        {
            Context.OnClickDetailButton.OnNext((new Model {OptionRow = _optionRow, SkillRow = _skillRow}, detailButton.transform));
        }
    }
}

#endif
