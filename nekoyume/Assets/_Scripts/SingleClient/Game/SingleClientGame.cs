using System;
using UnityEngine;

namespace Nekoyume.SingleClient.Game
{
    /// <summary>
    /// Lib9c-free game bootstrap singleton. Replaces the blockchain-dependent
    /// Nekoyume.Game.Game.cs for single-client mode.
    ///
    /// Access as <c>SingleClientGame.Instance</c> from runtime scripts.
    /// Composition order:
    ///   1. <see cref="SingleClientEntryPoint"/> runs first (RuntimeInitializeOnLoad).
    ///   2. <see cref="SingleClientGame"/> attaches to the same root GameObject
    ///      with a later execution order, wiring Runtime → TableSheets → Analytics.
    /// </summary>
    [DefaultExecutionOrder(-9000)]
    public sealed class SingleClientGame : MonoBehaviour
    {
        private static SingleClientGame _instance;
        public static SingleClientGame Instance => _instance;

        public SingleClientRuntime Runtime { get; private set; }
        public SingleClientTableSheets TableSheets { get; private set; }
        public SingleClientAnalytics Analytics { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoBoot()
        {
            if (_instance != null)
            {
                return;
            }

            var entry = SingleClientEntryPoint.Instance;
            GameObject host;
            if (entry != null)
            {
                host = entry.gameObject;
            }
            else
            {
                host = new GameObject(nameof(SingleClientGame));
                DontDestroyOnLoad(host);
            }

            _instance = host.AddComponent<SingleClientGame>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }

            _instance = this;

            var entry = SingleClientEntryPoint.Instance;
            Runtime = entry != null ? entry.Runtime : null;
            TableSheets = new SingleClientTableSheets();
            TableSheets.LoadFromResources();
            Analytics = new SingleClientAnalytics();

            Debug.Log($"[SingleClientGame] ready: runtime={(Runtime != null)}  sheets={TableSheets.Count}");
        }
    }
}
