#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Lib9cSubRecipeV2OptionInfo = Nekoyume.TableData.EquipmentItemSubRecipeSheetV2.OptionInfo;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cSubRecipeV2OptionInfo"/> into the client-owned
    /// <see cref="EquipmentItemSubRecipeSheetV2OptionInfoView"/>. Declared as an extension so
    /// the sub-recipe view can migrate call sites one <c>.ToView()</c> at a time.
    /// </summary>
    public static class EquipmentItemSubRecipeSheetV2OptionInfoMapper
    {
        public static EquipmentItemSubRecipeSheetV2OptionInfoView ToView(
            this Lib9cSubRecipeV2OptionInfo source)
        {
            return new EquipmentItemSubRecipeSheetV2OptionInfoView(
                id: source.Id,
                ratio: source.Ratio,
                requiredBlockIndex: source.RequiredBlockIndex);
        }
    }
}

#endif
