#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.Helper;
using Nekoyume.SingleClient.Models.TableData;
using Nekoyume.UI.Module.Common;
using UnityEngine;

namespace Nekoyume.UI.Scroller
{
    public class SummonSkillsCell : GridCell<SummonSkillsCell.Model, SummonSkillsScroll.ContextModel>
    {
        // Cell-local model stores client-owned row mirrors (<see cref="SkillSheetRowView"/> /
        // <see cref="EquipmentItemOptionRowView"/>) so downstream tooltip/rune consumers no
        // longer need to re-project lib9c sheet rows. The populator
        // (<c>SummonSkillsPopup.Show</c>) calls <c>.ToView()</c> at the sheet-access seam and
        // passes the views straight through; the rune overload receives
        // <see cref="SkillSheetRowView"/>? via the client-typed
        // <c>RuneOptionInfoMapper.ToView</c> overload added in S9a-ext.
        public class Model
        {
            public SummonDetailCell.Model SummonDetailCellModel;
            public SkillSheetRowView SkillRow;
            public EquipmentItemOptionRowView? EquipmentOptionRow;
        }

        [SerializeField]
        private SummonDetailCell summonDetailCell;

        [SerializeField]
        private SkillPositionTooltip skillView;

        public override void UpdateContent(Model itemData)
        {
            if (itemData.SummonDetailCellModel is not null)
            {
                summonDetailCell.UpdateContent(itemData.SummonDetailCellModel);
            }

            if (itemData.EquipmentOptionRow.HasValue)
            {
                skillView.Show(itemData.SkillRow, itemData.EquipmentOptionRow.Value);
            }

            if (itemData.SummonDetailCellModel?.RuneOptionInfo is not null)
            {
                var runeOptionInfo = itemData.SummonDetailCellModel.RuneOptionInfo;
                var runeValueString = RuneFrontHelper.GetRuneValueString(runeOptionInfo);
                // Model already holds a client-typed SkillSheetRowView — pass it to the
                // client-typed RuneOptionInfo.ToView(SkillSheetRowView?) overload so no
                // lib9c SkillSheet.Row ever re-enters this code path.
                skillView.Show(
                    itemData.SkillRow,
                    runeOptionInfo.ToView(itemData.SkillRow),
                    runeValueString);
            }
        }
    }
}

#endif
