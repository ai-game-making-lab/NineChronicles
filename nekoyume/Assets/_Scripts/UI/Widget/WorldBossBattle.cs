#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Battle;
using Nekoyume.Model;
using Nekoyume.SingleClient.Models.Buffs;
using Nekoyume.SingleClient.Models.EnumType;
using Nekoyume.SingleClient.State;
using Nekoyume.State;
using Nekoyume.UI.Module;
using UnityEngine;

namespace Nekoyume.UI
{
    public class WorldBossBattle : Widget
    {
        [SerializeField]
        private RaidBossStatus bossStatus;

        [SerializeField]
        private RaidPlayerStatus playerStatus;

        [SerializeField]
        private ComboText comboText;

        [SerializeField]
        private RaidProgressBar progressBar;

        protected override void Awake()
        {
            base.Awake();
            CloseWidget = null;
        }

        public void SetData(int bossId)
        {
            var turnLimit = 150;
            var sheet = Game.Game.instance.TableSheets.WorldBossCharacterSheet;
            if (sheet.TryGetValue(bossId, out var boss))
            {
                turnLimit = boss.WaveStats.FirstOrDefault().TurnLimit;
            }

            var (equipments, costumes) = ClientStateViewProvider.Current.GetEquippedItems(BattleType.Raid.ToLib9c());
            var level = ClientStateViewProvider.Current.CurrentAvatar?.Level ?? 0;
            comboText.comboMax = AttackCountHelper.GetCountMax(level);
            comboText.Close();
            playerStatus.SetData(equipments, costumes, turnLimit);
            progressBar.Clear(bossId);
        }

        public override void Show(bool ignoreShowAnimation = false)
        {
            base.Show(ignoreShowAnimation);
            progressBar.Show();
        }

        public void UpdateScore(long score)
        {
            progressBar.UpdateScore(score);
        }

        public void OnWaveCompleted()
        {
            progressBar.CompleteWave();
        }

        protected override void OnCompleteOfCloseAnimationInternal()
        {
            base.OnCompleteOfCloseAnimationInternal();
            progressBar.Close();
        }

        public void SetBossProfile(Enemy enemy, int turnLimit)
        {
            bossStatus.SetProfile(enemy);
            bossStatus.Show();
            playerStatus.UpdateTurnLimit(turnLimit);
        }

        public void UpdateStatus(
            long currentHp,
            long maxHp,
            Dictionary<int, Nekoyume.Model.Buff.Buff> buffs)
        {
            bossStatus.SetHp(currentHp, maxHp);
            bossStatus.SetBuff(buffs.ToViewMap());
        }

        public void ShowComboText(bool attacked)
        {
            comboText.StopAllCoroutines();
            comboText.Show(attacked);
        }
    }
}

#endif
