using System.Collections.Generic;

namespace Nekoyume.SingleClient
{
    public interface IClientRuntime
    {
        bool IsStarted { get; }

        ClientRuntimeState State { get; }

        ClientRuntimeState Start();

        ClientRuntimeState CreateOrSelectAvatar(string avatarId, string avatarName);

        ClientRuntimeState CreateOrSelectAvatar(int slotIndex, string avatarName);

        ClientRuntimeState SelectAvatar(int slotIndex);

        ClientRuntimeState ChargeActionPoint(long amount);

        ClientRuntimeState FillActionPoint(long actionPoint);

        ClientRuntimeState ConsumeActionPoint(long amount);

        ClientRuntimeState GrantInventoryItem(string itemId, long count);

        ClientRuntimeState ConsumeInventoryItem(string itemId, long count);

        ClientRuntimeState GrantEquipment(
            string nonFungibleId,
            int itemSheetId,
            int level = 0,
            long requiredBlockIndex = 0);

        ClientEnhanceResult EnhanceEquipment(
            string baseEquipmentId,
            IEnumerable<string> materialEquipmentIds,
            int levelDelta = 1);

        ClientGrindResult GrindEquipment(
            IEnumerable<string> equipmentIds,
            System.Numerics.BigInteger crystalGained,
            string crystalTicker = "CRYSTAL");

        System.Numerics.BigInteger GetCurrency(string ticker);

        ClientStagePlayPreview PreviewStagePlay(
            int stageId,
            long actionPointCost,
            string entryCostItemId = null,
            long entryCostItemCount = 0);

        ClientStagePlayPreview PreviewStagePlay(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts);

        ClientStagePlayResult PlayStage(
            int stageId,
            long actionPointCost,
            string entryCostItemId = null,
            long entryCostItemCount = 0);

        ClientStagePlayResult PlayStage(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts);

        ClientRuntimeState ClearStage(int stageId);

        ClientSweepResult SweepStage(
            int stageId,
            long actionPointCost,
            string apStoneItemId = null,
            long apStoneCount = 0,
            string entryCostItemId = null,
            long entryCostItemCount = 0);

        ClientSweepResult SweepStage(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts);

        long AdvanceBlock(long count = 1);

        void Save();
    }
}
