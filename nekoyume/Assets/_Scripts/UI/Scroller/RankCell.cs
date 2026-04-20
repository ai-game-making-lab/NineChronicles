using Nekoyume.SingleClient.State;
using Nekoyume.State;
using Nekoyume.UI.Model;
using UnityEngine;

namespace Nekoyume.UI.Scroller
{
    public class RankCell : RectCell<
        RankingModel,
        RankScroll.ContextModel>
    {
        [SerializeField]
        private RankCellPanel rankCell = null;

        [SerializeField]
        private RankCellPanel myInfoRankCell = null;

        public override void UpdateContent(RankingModel viewModel)
        {
            var currentAvatarAddress = ClientStateViewProvider.Current.CurrentAvatar?.Address;
            var isMyInfo = currentAvatarAddress is not null &&
                viewModel.AvatarAddress.Equals(currentAvatarAddress.Value.ToString());
            var cell = isMyInfo ? myInfoRankCell : rankCell;
            rankCell.gameObject.SetActive(!isMyInfo);
            myInfoRankCell.gameObject.SetActive(isMyInfo);
            cell.SetData(viewModel);
        }
    }
}
