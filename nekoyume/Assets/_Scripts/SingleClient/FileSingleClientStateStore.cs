using System;
using System.IO;
using UnityEngine;

namespace Nekoyume.SingleClient
{
    public sealed class FileSingleClientStateStore : ISingleClientStateStore
    {
        private readonly string _filePath;
        private readonly Func<DateTimeOffset> _clock;
        private readonly Func<string> _privateKeyFactory;

        public FileSingleClientStateStore(string filePath)
            : this(
                filePath,
                () => DateTimeOffset.UtcNow,
                SingleClientPrivateKeyFactory.CreatePrivateKeyHex)
        {
        }

        public FileSingleClientStateStore(string filePath, Func<DateTimeOffset> clock)
            : this(filePath, clock, SingleClientPrivateKeyFactory.CreatePrivateKeyHex)
        {
        }

        public FileSingleClientStateStore(
            string filePath,
            Func<DateTimeOffset> clock,
            Func<string> privateKeyFactory)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("State file path is required.", nameof(filePath));
            }

            _filePath = filePath;
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _privateKeyFactory = privateKeyFactory ??
                throw new ArgumentNullException(nameof(privateKeyFactory));
        }

        public bool Exists => File.Exists(_filePath);

        public string FilePath => _filePath;

        public SingleClientState LoadOrCreate()
        {
            if (!File.Exists(_filePath))
            {
                var defaultState = SingleClientState.CreateDefault(
                    _clock(),
                    _privateKeyFactory());
                Save(defaultState);
                return defaultState;
            }

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                var defaultState = SingleClientState.CreateDefault(
                    _clock(),
                    _privateKeyFactory());
                Save(defaultState);
                return defaultState;
            }

            var state = JsonUtility.FromJson<SingleClientState>(json);
            if (state is null)
            {
                throw new InvalidDataException($"Cannot parse single-client state: {_filePath}");
            }

            if (state.EnsureDefaults(_privateKeyFactory))
            {
                Save(state);
            }

            return state;
        }

        public void Save(SingleClientState state)
        {
            if (state is null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            state.EnsureDefaults(_privateKeyFactory);
            state.updatedAtUnixSeconds = _clock().ToUnixTimeSeconds();
            var directoryPath = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(_filePath, JsonUtility.ToJson(state, true));
        }
    }
}
