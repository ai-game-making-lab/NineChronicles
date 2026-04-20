using System;
using System.Collections.Generic;
using System.Linq;
using Nekoyume.Game.Character;
using Nekoyume.Game.Controller;
using Nekoyume.Game.Util;
using Nekoyume.SingleClient.State;
using Nekoyume.State;
using UnityEngine;
using mixpanel;
using Nekoyume.L10n;
using Nekoyume.Model.Mail;
using Nekoyume.SingleClient;
using Nekoyume.UI.Module;
using Nekoyume.UI.Scroller;

namespace Nekoyume.UI
{
    public class Login : Widget
    {
        [SerializeField]
        private GameObject[] slots = null;

        public bool ready;
        public List<Player> players;

        private ObjectPool _objectPool;

        protected override void Awake()
        {
            base.Awake();

            if (slots.Length != GameConfig.SlotCount)
            {
                throw new Exception("Login widget's slots.Length is not equals GameConfig.SlotCount.");
            }

            _objectPool = Game.Game.instance.Stage.ObjectPool;

            Game.Event.OnNestEnter.AddListener(ClearPlayers);
            Game.Lobby.OnLobbyEnterEvent += ClearPlayers;
            CloseWidget = null;
        }

        public void SlotClick(int index)
        {
            if (!ready)
            {
                return;
            }

            if (ClientStateViewProvider.Current.AvatarStatesRaw.TryGetValue(index, out var avatarState) &&
                (avatarState.inventory == null ||
                    avatarState.questList == null ||
                    avatarState.worldInformation == null))
            {
                NotificationSystem.Push(
                    MailType.System,
                    L10nManager.Localize("NOTIFICATION_CHARACTER_IS_BEING_RESTORED"),
                    NotificationCell.NotificationType.Alert);
                return;
            }

            Game.Event.OnLoginDetail.Invoke(index);
            gameObject.SetActive(false);
            AudioController.PlayClick();
        }

        public override void Show(bool ignoreShowAnimation = false)
        {
            base.Show(ignoreShowAnimation);
            Analyzer.Instance.Track("Unity/LoginImpression");

            if (!TryShowSingleClientSlots())
            {
                for (var i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    var playerSlot = slot.GetComponent<LoginPlayerSlot>();

                    if (ClientStateViewProvider.Current.AvatarStatesRaw.TryGetValue(i, out var avatarState))
                    {
                        playerSlot.LabelLevel.text = $"LV.{avatarState.level}";
                        playerSlot.LabelName.text = avatarState.NameWithHash;
                        playerSlot.CreateView.SetActive(false);
                        playerSlot.NameView.SetActive(true);
                    }
                    else
                    {
                        playerSlot.CreateView.SetActive(true);
                        playerSlot.NameView.SetActive(false);
                    }
                }
            }

            AudioController.instance.PlayMusic(AudioController.MusicCode.SelectCharacter);
        }

        private bool TryShowSingleClientSlots()
        {
            var game = Game.Game.instance;
            if (game is null ||
                !SingleClientMode.IsEnabled(game.CommandLineOptions) ||
                game.ClientRuntime?.State is not { } state)
            {
                return false;
            }

            for (var i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                var playerSlot = slot.GetComponent<LoginPlayerSlot>();
                var avatar = state.Avatars.FirstOrDefault(candidate => candidate.SlotIndex == i);
                if (avatar is null)
                {
                    playerSlot.CreateView.SetActive(true);
                    playerSlot.NameView.SetActive(false);
                    continue;
                }

                playerSlot.LabelLevel.text = $"LV.{avatar.Level}";
                playerSlot.LabelName.text = avatar.AvatarName;
                playerSlot.CreateView.SetActive(false);
                playerSlot.NameView.SetActive(true);
            }

            return true;
        }

        private void ClearPlayers()
        {
            foreach (var player in players)
            {
                player.DisableHUD();
                _objectPool.Remove<Player>(player.gameObject);
            }

            _objectPool.ReleaseAll();
            players.Clear();
        }
    }
}
