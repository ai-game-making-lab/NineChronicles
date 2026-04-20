using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Nekoyume.SingleClient;
using Nekoyume.SingleClient.Game;

namespace Nekoyume.Tests.PlayMode.SingleClient
{
    /// <summary>
    /// Unity Editor PlayMode verification for the Lib9c-free SingleClient
    /// bootstrap. Runs inside the Editor's Play state — equivalent to
    /// pressing the Play button and observing the console.
    /// </summary>
    public class SingleClientPlayModeTest
    {
        [UnityTest]
        public IEnumerator EntryPoint_AutoBoots_WithDefaultAvatar()
        {
            // Give BeforeSceneLoad hooks one frame to spawn the EntryPoint.
            yield return null;

            Assert.IsNotNull(SingleClientEntryPoint.Instance, "EntryPoint singleton missing");
            Assert.IsNotNull(SingleClientEntryPoint.Instance.Runtime, "Runtime not initialised");
            Assert.IsNotNull(SingleClientEntryPoint.Instance.Session, "Session not initialised");

            var state = SingleClientEntryPoint.Instance.Runtime.State;
            Assert.AreEqual("local-player", state.PlayerId);
            Assert.AreEqual("local-avatar", state.AvatarId);
            Assert.AreEqual("Local Avatar", state.AvatarName);
            Assert.GreaterOrEqual(state.AvatarLevel, 1);
        }

        [UnityTest]
        public IEnumerator Game_Singleton_WiresRuntimeAndSheetsAndAnalytics()
        {
            yield return null;

            Assert.IsNotNull(SingleClientGame.Instance, "SingleClientGame singleton missing");
            Assert.IsNotNull(SingleClientGame.Instance.Runtime, "Game.Runtime not wired");
            Assert.IsNotNull(SingleClientGame.Instance.TableSheets, "Game.TableSheets not wired");
            Assert.IsNotNull(SingleClientGame.Instance.Analytics, "Game.Analytics not wired");
        }

        [UnityTest]
        public IEnumerator StartingLoadout_FillsActionPoint()
        {
            yield return null;

            var entry = SingleClientEntryPoint.Instance;
            Assert.IsNotNull(entry);

            // Reset to 0 first so the test is order-independent (play-mode session
            // retains state across tests; an earlier GrantStartingLoadout may have
            // already brought AP up to 120).
            var current = entry.Runtime.State.ActionPoint;
            if (current > 0)
            {
                entry.Runtime.ConsumeActionPoint(current);
            }

            Assert.AreEqual(0L, entry.Runtime.State.ActionPoint);
            entry.GrantStartingLoadout();
            var afterAp = entry.Runtime.State.ActionPoint;

            Assert.GreaterOrEqual(afterAp, 120L, $"ActionPoint not filled to 120: {afterAp}");
        }

        [UnityTest]
        public IEnumerator Analytics_TrackDoesNotThrow()
        {
            yield return null;

            var analytics = SingleClientGame.Instance?.Analytics;
            Assert.IsNotNull(analytics);

            analytics.LoggingEnabled = false;
            Assert.DoesNotThrow(() => analytics.Track("PlayModeTest.smoke"));
            Assert.DoesNotThrow(() => analytics.Track(
                "PlayModeTest.withProps",
                new System.Collections.Generic.Dictionary<string, object>
                {
                    ["k1"] = 1,
                    ["k2"] = "value",
                }));
        }
    }
}
