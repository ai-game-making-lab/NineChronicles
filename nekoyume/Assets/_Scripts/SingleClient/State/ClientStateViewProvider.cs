#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Lib9cStates = Nekoyume.State.States;

namespace Nekoyume.SingleClient.State
{
    /// <summary>
    /// Static entry point for resolving the current <see cref="IClientStateView"/> instance.
    /// Lets consumers adopt the facade before VContainer DI wiring reaches them — migrating
    /// callers replace <c>States.Instance.CurrentAvatarState</c>-style reads with
    /// <c>ClientStateViewProvider.Current.CurrentAvatar</c>-style reads.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Current"/> defaults to a <see cref="LegacyStatesAdapter"/> bound to
    /// <c>States.Instance</c>. Tests inject a test-double via <see cref="Override"/> and
    /// restore the default via <see cref="Reset"/>.
    /// </para>
    /// <para>
    /// Deliberately NOT thread-safe: the Unity main-thread is the only writer, matching the
    /// access pattern of the legacy <c>States.Instance</c> it wraps. If the single-client
    /// runtime grows background-thread state producers, this provider will need to move to
    /// <c>Interlocked.Exchange</c> (or, preferably, to VContainer injection).
    /// </para>
    /// </remarks>
    public static class ClientStateViewProvider
    {
        private static IClientStateView _override;

        /// <summary>
        /// Active view. Defaults to a lazily-built <see cref="LegacyStatesAdapter"/> wrapping
        /// <c>States.Instance</c>. Lazy construction avoids triggering
        /// <c>ReactiveAvatarState</c> initialization (and Unity runtime state) at static-ctor
        /// time, which would fail in headless EditMode tests.
        /// </summary>
        public static IClientStateView Current =>
            _override ?? _defaultLazy.Value;

        private static System.Lazy<IClientStateView> _defaultLazy =
            new System.Lazy<IClientStateView>(() => new LegacyStatesAdapter(Lib9cStates.Instance));

        /// <summary>
        /// Swap in a test double or an alternate implementation. Pass <see langword="null"/>
        /// to intentionally clear the provider (callers will NRE on the next read — this mode
        /// is deliberate for misuse detection in tests).
        /// </summary>
        public static void Override(IClientStateView @override)
        {
            _override = @override;
        }

        /// <summary>
        /// Restore the default <see cref="LegacyStatesAdapter"/>. Also disposes the previously
        /// overridden view when it implements <see cref="System.IDisposable"/> so test-double
        /// subscriptions don't leak across tests.
        /// </summary>
        public static void Reset()
        {
            if (_override is System.IDisposable disposable)
            {
                disposable.Dispose();
            }

            _override = null;
        }
    }
}

#endif
