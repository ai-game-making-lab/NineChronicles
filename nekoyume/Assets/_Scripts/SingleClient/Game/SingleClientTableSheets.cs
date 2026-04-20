using System.Collections.Generic;
using UnityEngine;

namespace Nekoyume.SingleClient.Game
{
    /// <summary>
    /// Lib9c-free table sheet loader. Replaces the blockchain-synced
    /// <c>Nekoyume.Game.TableSheets</c> with a Resources-backed local loader.
    ///
    /// Load target: any <see cref="TextAsset"/> placed under
    /// <c>Resources/single-client/sheets/</c>. Each file is cached by filename
    /// (stem, case-sensitive) as raw text — callers parse as JSON / CSV / custom.
    /// </summary>
    public sealed class SingleClientTableSheets
    {
        private const string ResourcesPrefix = "single-client/sheets";

        private readonly Dictionary<string, string> _byName = new();

        public int Count => _byName.Count;

        public IReadOnlyDictionary<string, string> RawByName => _byName;

        public void LoadFromResources()
        {
            _byName.Clear();
            var assets = Resources.LoadAll<TextAsset>(ResourcesPrefix);
            if (assets == null)
            {
                return;
            }

            foreach (var asset in assets)
            {
                if (asset == null || string.IsNullOrEmpty(asset.name))
                {
                    continue;
                }

                _byName[asset.name] = asset.text ?? string.Empty;
            }
        }

        public bool TryGetRaw(string sheetName, out string raw)
        {
            return _byName.TryGetValue(sheetName, out raw);
        }

        public string GetRawOrDefault(string sheetName, string fallback = "")
        {
            return _byName.TryGetValue(sheetName, out var raw) ? raw : fallback;
        }
    }
}
