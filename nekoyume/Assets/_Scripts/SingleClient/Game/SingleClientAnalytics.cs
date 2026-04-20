using System.Collections.Generic;
using UnityEngine;

namespace Nekoyume.SingleClient.Game
{
    /// <summary>
    /// Lib9c-free analytics facade. Replaces <c>Nekoyume.Analyzer</c> /
    /// mixpanel / sentry integrations with a no-op local logger that emits
    /// to <see cref="Debug.Log"/> only.
    ///
    /// Kept API-shape minimal so callers can adopt it in stages: Track(name),
    /// Track(name, properties). Future expansion can hook to local on-disk
    /// JSON lines or an opt-in telemetry endpoint without lib9c coupling.
    /// </summary>
    public sealed class SingleClientAnalytics
    {
        public bool LoggingEnabled { get; set; } = true;

        public void Track(string eventName)
        {
            if (!LoggingEnabled || string.IsNullOrEmpty(eventName))
            {
                return;
            }

            Debug.Log($"[SCAnalytics] {eventName}");
        }

        public void Track(string eventName, IReadOnlyDictionary<string, object> properties)
        {
            if (!LoggingEnabled || string.IsNullOrEmpty(eventName))
            {
                return;
            }

            if (properties == null || properties.Count == 0)
            {
                Debug.Log($"[SCAnalytics] {eventName}");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.Append($"[SCAnalytics] {eventName} {{");
            var first = true;
            foreach (var kv in properties)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;
                sb.Append(kv.Key).Append('=').Append(kv.Value);
            }
            sb.Append('}');
            Debug.Log(sb.ToString());
        }
    }
}
