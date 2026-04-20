#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Libplanet.Types.Assets;
using Nekoyume.SingleClient.Models.Items;
using Nekoyume.UI.Tween;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    public class GrindReward : MonoBehaviour
    {
        private class RewardType
        {
            public string FavTicker;
            public int ItemId;
            public bool IsButton;

            public RewardType()
            {
                Reset();
            }

            public void Reset()
            {
                FavTicker = null;
                ItemId = 0;
                IsButton = false;
            }
        }

        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private DigitTextTweener rewardTweener;

        [SerializeField]
        private Button moreInfoButton;

        [SerializeField]
        private Animator animator;

        private static readonly int Show = Animator.StringToHash("Show");
        private readonly RewardType _cachedRewardType = new RewardType();
        private long _cachedGrindingReward;

        public void ShowWithFavReward(FungibleAssetValue reward)
        {
            gameObject.SetActive(true);
            if (reward.Currency.Ticker != _cachedRewardType.FavTicker)
            {
                _cachedRewardType.FavTicker = reward.Currency.Ticker;
                _cachedGrindingReward = 0;
                // To avoid GrindModule animation conflict
                Observable.NextFrame().Subscribe(_ => animator.SetTrigger(Show));
            }

            iconImage.gameObject.SetActive(true);
            rewardTweener.gameObject.SetActive(true);
            moreInfoButton.gameObject.SetActive(false);

            iconImage.sprite = reward.GetIconSprite();
            var prevReward = _cachedGrindingReward;
            _cachedGrindingReward = (long)reward.MajorUnit;
            rewardTweener.PlayWithNotation(prevReward, _cachedGrindingReward);
        }

        /// <summary>
        /// Snapshot-facing primary overload. Callers that still hold a lib9c <c>ItemBase</c>
        /// should project via <c>ItemSnapshotMapper.ToPolySnapshot()</c> at the call site; this
        /// view no longer depends on <c>Nekoyume.Model.Item</c> types.
        /// </summary>
        public void ShowWithItemReward((IItemSnapshot snapshot, int count) reward)
        {
            gameObject.SetActive(true);
            if (reward.snapshot.Id != _cachedRewardType.ItemId)
            {
                _cachedRewardType.ItemId = reward.snapshot.Id;
                _cachedGrindingReward = 0;
                Observable.NextFrame().Subscribe(_ => animator.SetTrigger(Show));
            }

            iconImage.gameObject.SetActive(true);
            rewardTweener.gameObject.SetActive(true);
            moreInfoButton.gameObject.SetActive(false);

            iconImage.sprite = reward.snapshot.GetIconSprite();
            var prevReward = _cachedGrindingReward;
            _cachedGrindingReward = reward.count;
            rewardTweener.PlayWithNotation(prevReward, _cachedGrindingReward);
        }

        public void ShowWithButton(UnityAction onClick)
        {
            gameObject.SetActive(true);
            if (!_cachedRewardType.IsButton)
            {
                _cachedRewardType.IsButton = true;
                Observable.NextFrame().Subscribe(_ => animator.SetTrigger(Show));
            }

            iconImage.gameObject.SetActive(false);
            rewardTweener.gameObject.SetActive(false);
            moreInfoButton.gameObject.SetActive(true);

            moreInfoButton.onClick.AddListener(onClick);
        }

        public void HideAndReset()
        {
            gameObject.SetActive(false);
            _cachedRewardType.Reset();
        }
    }
}

#endif
