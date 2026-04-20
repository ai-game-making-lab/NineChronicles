using System;
using System.Collections.Generic;

namespace Nekoyume.SingleClient
{
    public sealed class SingleClientRuntime : IClientRuntime
    {
        private readonly SingleClientSession _session;

        public SingleClientRuntime(SingleClientSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public bool IsStarted => _session.IsStarted;

        public ClientRuntimeState State =>
            _session.State is null ? null : new ClientRuntimeState(_session.State);

        public ClientRuntimeState Start()
        {
            return new ClientRuntimeState(_session.Start());
        }

        public ClientRuntimeState CreateOrSelectAvatar(string avatarId, string avatarName)
        {
            return new ClientRuntimeState(_session.CreateOrSelectAvatar(avatarId, avatarName));
        }

        public ClientRuntimeState CreateOrSelectAvatar(int slotIndex, string avatarName)
        {
            return new ClientRuntimeState(_session.CreateOrSelectAvatar(slotIndex, avatarName));
        }

        public ClientRuntimeState SelectAvatar(int slotIndex)
        {
            return new ClientRuntimeState(_session.SelectAvatar(slotIndex));
        }

        public ClientRuntimeState ChargeActionPoint(long amount)
        {
            return new ClientRuntimeState(_session.ChargeActionPoint(amount));
        }

        public ClientRuntimeState FillActionPoint(long actionPoint)
        {
            return new ClientRuntimeState(_session.FillActionPoint(actionPoint));
        }

        public ClientRuntimeState ConsumeActionPoint(long amount)
        {
            return new ClientRuntimeState(_session.ConsumeActionPoint(amount));
        }

        public ClientRuntimeState GrantInventoryItem(string itemId, long count)
        {
            return new ClientRuntimeState(_session.GrantInventoryItem(itemId, count));
        }

        public ClientRuntimeState ConsumeInventoryItem(string itemId, long count)
        {
            return new ClientRuntimeState(_session.ConsumeInventoryItem(itemId, count));
        }

        public ClientRuntimeState GrantEquipment(
            string nonFungibleId,
            int itemSheetId,
            int level = 0,
            long requiredBlockIndex = 0)
        {
            _session.GrantEquipment(nonFungibleId, itemSheetId, level, requiredBlockIndex);
            return new ClientRuntimeState(_session.State);
        }

        public ClientEnhanceResult EnhanceEquipment(
            string baseEquipmentId,
            IEnumerable<string> materialEquipmentIds,
            int levelDelta = 1)
        {
            return new ClientEnhanceResult(_session.EnhanceEquipment(
                baseEquipmentId,
                materialEquipmentIds,
                levelDelta));
        }

        public ClientGrindResult GrindEquipment(
            IEnumerable<string> equipmentIds,
            System.Numerics.BigInteger crystalGained,
            string crystalTicker = "CRYSTAL")
        {
            return new ClientGrindResult(_session.GrindEquipment(
                equipmentIds,
                crystalGained,
                crystalTicker));
        }

        public System.Numerics.BigInteger GetCurrency(string ticker)
        {
            return _session.GetCurrency(ticker);
        }

        public ClientStagePlayPreview PreviewStagePlay(
            int stageId,
            long actionPointCost,
            string entryCostItemId = null,
            long entryCostItemCount = 0)
        {
            return new ClientStagePlayPreview(_session.PreviewStagePlay(
                stageId,
                actionPointCost,
                entryCostItemId,
                entryCostItemCount));
        }

        public ClientStagePlayPreview PreviewStagePlay(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts)
        {
            return new ClientStagePlayPreview(_session.PreviewStagePlay(
                stageId,
                actionPointCost,
                itemCosts));
        }

        public ClientStagePlayResult PlayStage(
            int stageId,
            long actionPointCost,
            string entryCostItemId = null,
            long entryCostItemCount = 0)
        {
            return new ClientStagePlayResult(_session.PlayStage(
                stageId,
                actionPointCost,
                entryCostItemId,
                entryCostItemCount));
        }

        public ClientStagePlayResult PlayStage(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts)
        {
            return new ClientStagePlayResult(_session.PlayStage(
                stageId,
                actionPointCost,
                itemCosts));
        }

        public ClientRuntimeState ClearStage(int stageId)
        {
            return new ClientRuntimeState(_session.ClearStage(stageId));
        }

        public ClientSweepResult SweepStage(
            int stageId,
            long actionPointCost,
            string apStoneItemId = null,
            long apStoneCount = 0,
            string entryCostItemId = null,
            long entryCostItemCount = 0)
        {
            return new ClientSweepResult(_session.SweepStage(
                stageId,
                actionPointCost,
                apStoneItemId,
                apStoneCount,
                entryCostItemId,
                entryCostItemCount));
        }

        public ClientSweepResult SweepStage(
            int stageId,
            long actionPointCost,
            IEnumerable<ClientItemCost> itemCosts)
        {
            return new ClientSweepResult(_session.SweepStage(
                stageId,
                actionPointCost,
                itemCosts));
        }

        public long AdvanceBlock(long count = 1)
        {
            return _session.AdvanceBlock(count);
        }

        public void Save()
        {
            _session.Save();
        }
    }
}
