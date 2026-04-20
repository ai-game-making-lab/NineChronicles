using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Nekoyume.SingleClient.UI
{
    /// <summary>
    /// Minimal Lib9c-free main-menu widget. Spawns three buttons:
    ///   Start Game • Avatar Info • Quit
    /// Behavior delegates to <see cref="SingleClient.SingleClientEntryPoint"/>.
    ///
    /// The UI is created programmatically via
    /// <see cref="RuntimeInitializeOnLoadMethod"/> so no scene asset changes
    /// are required. A future Phase can replace this with a proper prefab.
    /// </summary>
    public sealed class SingleClientMainMenuWidget : MonoBehaviour
    {
        private static SingleClientMainMenuWidget _instance;
        public static SingleClientMainMenuWidget Instance => _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBoot()
        {
            if (_instance != null)
            {
                return;
            }

            var canvasGo = new GameObject("SingleClientMainMenuCanvas");
            DontDestroyOnLoad(canvasGo);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            _instance = canvasGo.AddComponent<SingleClientMainMenuWidget>();
            _instance.BuildUI();
        }

        private GameObject _root;
        private TextMeshProUGUI _statusText;

        private void BuildUI()
        {
            _root = new GameObject("MainMenuRoot");
            _root.transform.SetParent(transform, false);
            var rt = _root.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(400, 320);

            var vlg = _root.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8f;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            _statusText = CreateText("NineChronicles — SingleClient Mode");
            CreateButton("Start Game", OnStartGameClicked);
            CreateButton("Avatar Info", OnAvatarInfoClicked);
            CreateButton("Quit", OnQuitClicked);
        }

        private TextMeshProUGUI CreateText(string initial)
        {
            var go = new GameObject("StatusText");
            go.transform.SetParent(_root.transform, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = initial;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 20;
            return tmp;
        }

        private void CreateButton(string label, UnityEngine.Events.UnityAction handler)
        {
            var go = new GameObject($"Button_{label}");
            go.transform.SetParent(_root.transform, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.15f, 0.18f, 0.22f, 0.85f);
            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(handler);

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 48f;

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontSize = 20;
            var lrt = tmp.rectTransform;
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
        }

        private void OnStartGameClicked()
        {
            var entry = SingleClient.SingleClientEntryPoint.Instance;
            if (entry == null)
            {
                _statusText.text = "Entry point missing";
                return;
            }

            entry.GrantStartingLoadout();
            entry.LogState("StartGame clicked");

            var state = entry.Runtime.State;
            _statusText.text = $"Started — {state.AvatarName} (slot {state.AvatarSlotIndex}) AP:{state.ActionPoint}";
        }

        private void OnAvatarInfoClicked()
        {
            var entry = SingleClient.SingleClientEntryPoint.Instance;
            if (entry == null)
            {
                _statusText.text = "Entry point missing";
                return;
            }

            var state = entry.Runtime.State;
            _statusText.text =
                $"Avatar: {state.AvatarName}\n" +
                $"Level: {state.AvatarLevel}  AP: {state.ActionPoint}\n" +
                $"Stage cleared: {state.HighestClearedStageId}\n" +
                $"Block: {state.BlockIndex}";
            entry.LogState("AvatarInfo clicked");
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
