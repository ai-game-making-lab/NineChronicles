#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Linq;
using Nekoyume.SingleClient.State;
using Nekoyume.State;
using Nekoyume.Game.Controller;
using Nekoyume.Model;
using UnityEngine;
using System.Text.RegularExpressions;
using Nekoyume.UI.Module;
using TMPro;
using UnityEngine.UI;
using Nekoyume.Model.State;
using System.Collections;
using System.Collections.Generic;
using Nekoyume.Game;
using Nekoyume.Helper;
using Nekoyume.L10n;
using Nekoyume.SingleClient;
using Nekoyume.SingleClient.Models.EnumType;

namespace Nekoyume.UI
{
    using Nekoyume.Model.Stat;
    using UniRx;

    public class LoginDetail : Widget
    {
        public GameObject btnLogin;
        public GameObject btnCreate;
        public TextMeshProUGUI levelAndNameInfo;

        public GameObject jobInfoContainer;
        public TextMeshProUGUI jobDescriptionText;
        public GameObject statusContainer;
        public DetailedStatView[] statusRows;
        public LoginDetailCostume loginDetailCostume;

        public Button backButton;
        public TextMeshProUGUI backButtonText;

        private readonly HashSet<StatType> visibleStats = new()
        {
            StatType.HP,
            StatType.ATK,
            StatType.DEF,
            StatType.CRI,
            StatType.HIT,
            StatType.SPD
        };

        private int _selectedIndex;
        private bool _isCreateMode;

        protected override void Awake()
        {
            base.Awake();

            jobDescriptionText.text = L10nManager.Localize("UI_WARRIOR_DESCRIPTION");

            Game.Event.OnLoginDetail.AddListener(Init);

            CloseWidget = BackClick;
            SubmitWidget = CreateClick;

            backButton.OnClickAsObservable()
                .ThrottleFirst(new TimeSpan(0, 0, 1))
                .Subscribe(_ => BackClick())
                .AddTo(gameObject);
        }

        public void CreateClick()
        {
            AudioController.PlayClick();
            Analyzer.Instance.Track("Unity/Create Click");

            var inputBox = Find<InputBoxPopup>();
            inputBox.CloseCallback = result =>
            {
                if (result == ConfirmResult.Yes)
                {
                    CreateAndLogin(inputBox.text);
                }
            };
            inputBox.Show("UI_INPUT_NAME", "UI_NICKNAME_CONDITION");
        }

        private void CreateAndLogin(string nickName)
        {
            if (!Regex.IsMatch(nickName, GameConfig.AvatarNickNamePattern))
            {
                Find<Alert>().Show("UI_ERROR", "UI_NICKNAME_CONDITION");
                return;
            }

            Analyzer.Instance.Track("Unity/Choose Nickname");

            Find<LoadingScreen>().Show(
                LoadingScreen.LoadingType.Entering,
                L10nManager.Localize("UI_IN_MINING_A_BLOCK"));
            if (TryCreateAndLoginSingleClient(nickName))
            {
                return;
            }

            var (earIndex, tailIndex, hairIndex, eyeIndex) = loginDetailCostume.GetCostumeId();
            Game.Game.instance.ActionManager
                .CreateAvatar(_selectedIndex, nickName, hairIndex, eyeIndex, earIndex, tailIndex)
                .DoOnError(e =>
                {
                    Game.Game.PopupError(e).Forget();
                    Find<LoadingScreen>().Close();
                })
                .Subscribe();
        }

        public void OnRenderCreateAvatar()
        {
            StartCoroutine(CreateAndLoginAnimation());
        }

        private IEnumerator CreateAndLoginAnimation()
        {
            var loadingScreen = Find<LoadingScreen>();
            if (loadingScreen is null)
            {
                yield break;
            }

            loadingScreen.Close();
            yield return new WaitUntil(() => loadingScreen.IsCloseAnimationCompleted);
            OnDidAvatarStateLoaded();
        }

        public async void LoginClick()
        {
            AudioController.PlayClick();
            btnLogin.SetActive(false);
            var loadingScreen = Find<LoadingScreen>();
            loadingScreen.Show(
                LoadingScreen.LoadingType.Entering, L10nManager.Localize("UI_IN_MINING_A_BLOCK"));
            if (TryLoginSingleClient(loadingScreen))
            {
                return;
            }

            await RxProps.SelectAvatarAsync(_selectedIndex, Game.Game.instance.Agent.BlockTipStateRootHash);
            loadingScreen.Close();
            OnDidAvatarStateLoaded();
        }

        public void BackToLogin()
        {
            Close();
            Game.Event.OnNestEnter.Invoke();
            var login = Find<Login>();
            login.Show();
        }

        private async void Init(int index)
        {
            _selectedIndex = index;
            if (TryInitSingleClient(index))
            {
                return;
            }

            Player player;
            _isCreateMode = !ClientStateViewProvider.Current.AvatarStatesRaw.ContainsKey(index);
            var tableSheets = Game.Game.instance.TableSheets;

            if (_isCreateMode)
            {
                player = new Player(1, tableSheets.CharacterSheet, tableSheets.CharacterLevelSheet,
                    tableSheets.EquipmentItemSetEffectSheet);
            }
            else
            {
                var loadingScreen = Find<LoadingScreen>();
                loadingScreen.Show(
                    LoadingScreen.LoadingType.JustModule,
                    L10nManager.Localize("UI_LOADING_BOOTSTRAP_START"));
                await ClientStateViewProvider.Current.SelectAvatarAsync(_selectedIndex, Game.Game.instance.Agent.BlockTipStateRootHash);
                Game.Event.OnUpdateAddresses.Invoke();
                loadingScreen.Close();
                player = new Player(
                    ClientStateViewProvider.Current.CurrentAvatarStateRaw,
                    tableSheets.CharacterSheet,
                    tableSheets.CharacterLevelSheet,
                    tableSheets.EquipmentItemSetEffectSheet
                );

                var runeStates = ClientStateViewProvider.Current.GetEquippedRuneStates(BattleType.Adventure.ToLib9c());

                var allRuneState = ClientStateViewProvider.Current.AllRuneStateRaw;
                var runeListSheet = tableSheets.RuneListSheet;
                var runeLevelBonusSheet = tableSheets.RuneLevelBonusSheet;
                var runeLevelBonus = RuneHelper.CalculateRuneLevelBonus(
                    allRuneState, runeListSheet, runeLevelBonusSheet);

                var costumeStatSheet = Game.Game.instance.TableSheets.CostumeStatSheet;
                var collectionState = ClientStateViewProvider.Current.CollectionStateRaw;
                var collectionSheet = Game.Game.instance.TableSheets.CollectionSheet;
                player.ConfigureStats(
                    costumeStatSheet,
                    runeStates,
                    tableSheets.RuneOptionSheet,
                    runeLevelBonus,
                    tableSheets.SkillSheet,
                    collectionState.GetEffects(collectionSheet));
            }

            // create new or login
            btnCreate.SetActive(_isCreateMode);
            loginDetailCostume.SetActive(_isCreateMode);

            // 프로필 사진의 용도가 정리되지 않아서 주석 처리함.
            // profileImage.SetActive(!isCreateMode);
            btnLogin.SetActive(!_isCreateMode);
            jobInfoContainer.SetActive(!_isCreateMode);
            levelAndNameInfo.gameObject.SetActive(!_isCreateMode);
            statusContainer.SetActive(!_isCreateMode);
            if (!_isCreateMode)
            {
                var level = player.Level;
                var name = ClientStateViewProvider.Current.CurrentAvatarStateRaw.NameWithHash;
                levelAndNameInfo.text = $"LV. {level} {name}";
                SetInformation(player);
            }

            backButtonText.text = _isCreateMode ? L10nManager.Localize("UI_CHARACTER_CREATE") : "";

            if (_isCreateMode)
            {
                SubmitWidget = CreateClick;
            }
            else
            {
                SubmitWidget = LoginClick;
            }

            Show();
        }

        private bool TryInitSingleClient(int index)
        {
            if (!TryGetSingleClientRuntime(out var runtime) ||
                runtime.State is not { } state)
            {
                return false;
            }

            var avatar = state.Avatars.FirstOrDefault(candidate => candidate.SlotIndex == index);
            _isCreateMode = avatar is null;
            var tableSheets = Game.Game.instance.TableSheets;
            var level = _isCreateMode ? 1 : Math.Max(1, avatar.Level);
            var player = new Player(
                level,
                tableSheets.CharacterSheet,
                tableSheets.CharacterLevelSheet,
                tableSheets.EquipmentItemSetEffectSheet);

            btnCreate.SetActive(_isCreateMode);
            loginDetailCostume.SetActive(_isCreateMode);
            btnLogin.SetActive(!_isCreateMode);
            jobInfoContainer.SetActive(!_isCreateMode);
            levelAndNameInfo.gameObject.SetActive(!_isCreateMode);
            statusContainer.SetActive(!_isCreateMode);
            if (!_isCreateMode)
            {
                levelAndNameInfo.text = $"LV. {player.Level} {avatar.AvatarName}";
                SetInformation(player);
            }

            backButtonText.text = _isCreateMode ? L10nManager.Localize("UI_CHARACTER_CREATE") : "";
            SubmitWidget = _isCreateMode ? CreateClick : LoginClick;
            Show();
            return true;
        }

        private bool TryCreateAndLoginSingleClient(string nickName)
        {
            if (!TryGetSingleClientRuntime(out var runtime))
            {
                return false;
            }

            try
            {
                runtime.CreateOrSelectAvatar(_selectedIndex, nickName);
                OnRenderCreateAvatar();
            }
            catch (Exception e)
            {
                Game.Game.PopupError(e).Forget();
                Find<LoadingScreen>().Close();
            }

            return true;
        }

        private bool TryLoginSingleClient(LoadingScreen loadingScreen)
        {
            if (!TryGetSingleClientRuntime(out var runtime))
            {
                return false;
            }

            try
            {
                runtime.SelectAvatar(_selectedIndex);
                loadingScreen.Close();
                OnDidAvatarStateLoaded();
            }
            catch (Exception e)
            {
                Game.Game.PopupError(e).Forget();
                loadingScreen.Close();
                btnLogin.SetActive(true);
            }

            return true;
        }

        private static bool TryGetSingleClientRuntime(out IClientRuntime runtime)
        {
            runtime = null;
            var game = Game.Game.instance;
            if (game is null || !SingleClientMode.IsEnabled(game.CommandLineOptions))
            {
                return false;
            }

            runtime = game.ClientRuntime;
            return runtime is not null;
        }

        private void SetInformation(Player player)
        {
            var idx = 0;
            foreach (var (statType, value) in player.Stats.GetStats())
            {
                if (!visibleStats.Contains(statType))
                {
                    continue;
                }

                var info = statusRows[idx];
                info.Show(statType, value, 0);
                ++idx;
            }
        }

        public override void Show(bool ignoreShowAnimation = false)
        {
            Analyzer.Instance.Track("Unity/CustomizeAvatar/Show");

            base.Show(ignoreShowAnimation);
            if (_isCreateMode)
            {
                loginDetailCostume.Show();
            }
        }

        private void BackClick()
        {
            BackToLogin();
        }

        private void OnDidAvatarStateLoaded()
        {
            Util.SaveAvatarSlotIndex(_selectedIndex);
            if (_isCreateMode)
            {
                Close();
            }

            EnterLobby();
        }

        private void EnterLobby()
        {
            Close();
            Lobby.Enter();
            Game.Event.OnUpdateAddresses.Invoke();
        }
    }
}

#endif
