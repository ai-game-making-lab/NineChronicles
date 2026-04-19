using Nekoyume.Helper;
using Nekoyume.SingleClient.Models.TableData;
using Nekoyume.TableData;
using Nekoyume.UI.Module.Common;
using UnityEngine;

namespace Nekoyume.UI.Scroller
{
    public class SummonSkillsCell : GridCell<SummonSkillsCell.Model, SummonSkillsScroll.ContextModel>
    {
        public class Model
        {
            public SummonDetailCell.Model SummonDetailCellModel;
            public SkillSheet.Row SkillRow;
            public EquipmentItemOptionSheet.Row EquipmentOptionRow;
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

            if (itemData.EquipmentOptionRow is not null)
            {
                skillView.Show(itemData.SkillRow.ToView(), itemData.EquipmentOptionRow.ToView());
            }

            if (itemData.SummonDetailCellModel?.RuneOptionInfo is not null)
            {
                var runeOptionInfo = itemData.SummonDetailCellModel.RuneOptionInfo;
                var runeValueString = RuneFrontHelper.GetRuneValueString(runeOptionInfo);
                skillView.Show(
                    itemData.SkillRow.ToView(),
                    runeOptionInfo.ToView(itemData.SkillRow),
                    runeValueString);
            }
        }
    }
}
