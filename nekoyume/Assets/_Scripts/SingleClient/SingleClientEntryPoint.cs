using UnityEngine;

namespace Nekoyume.SingleClient
{
    /// <summary>
    /// Minimal Lib9c-free runtime entry point. Replaces the blockchain-dependent
    /// Game.cs bootstrap for single-client mode. Auto-spawns on first scene load,
    /// creates a local state store, starts a session, and logs the avatar/balance
    /// state to console.
    ///
    /// Drop this into any scene that's referenced as an active Build Scene
    /// (e.g. via BuildSettings) to smoke-test the SingleClient infrastructure
    /// without any Game.cs / ActionManager / UI coupling.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public sealed class SingleClientEntryPoint : MonoBehaviour
    {
        private static SingleClientEntryPoint _instance;
        public static SingleClientEntryPoint Instance => _instance;

        public ISingleClientStateStore StateStore { get; private set; }
        public SingleClientSession Session { get; private set; }
        public SingleClientRuntime Runtime { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoBoot()
        {
            if (_instance != null)
            {
                return;
            }

            var go = new GameObject(nameof(SingleClientEntryPoint));
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<SingleClientEntryPoint>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            var path = SingleClientPaths.GetDefaultStatePath();
            StateStore = new FileSingleClientStateStore(path);
            Session = new SingleClientSession(StateStore);
            Runtime = new SingleClientRuntime(Session);
            Runtime.Start();

            LogState("SingleClient bootstrap complete");
        }

        public void GrantStartingLoadout()
        {
            Runtime.FillActionPoint(120);
            Runtime.GrantInventoryItem("starter-weapon", 1);
            Runtime.GrantInventoryItem("starter-potion", 5);
            LogState("Starting loadout granted");
        }

        public void LogState(string header)
        {
            var state = Runtime.State;
            Debug.Log($"[SingleClient] {header}");
            Debug.Log($"  player: {state.PlayerId}");
            Debug.Log($"  avatar: {state.AvatarId} / {state.AvatarName} (slot {state.AvatarSlotIndex})");
            Debug.Log($"  level: {state.AvatarLevel}  actionPoint: {state.ActionPoint}");
            Debug.Log($"  highestClearedStage: {state.HighestClearedStageId}");
            Debug.Log($"  blockIndex: {state.BlockIndex}  updatedAt(unix s): {state.UpdatedAtUnixSeconds}");
        }
    }
}
