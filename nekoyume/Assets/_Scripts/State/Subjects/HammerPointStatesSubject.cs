#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.State;
﻿using System;
using Nekoyume.Model.State;
using UniRx;

namespace Nekoyume.State.Subjects
{
    public static class HammerPointStatesSubject
    {
        private static readonly Subject<(int, HammerPointState)> HammerPointSubjectInternal;

        public static readonly IObservable<(int, HammerPointState)> HammerPoint;

        static HammerPointStatesSubject()
        {
            HammerPointSubjectInternal = new Subject<(int, HammerPointState)>();
            HammerPoint = HammerPointSubjectInternal.ObserveOnMainThread();
        }

        public static void OnReplaceHammerPointState(int recipeId, HammerPointState state)
        {
            if (Addresses.GetHammerPointStateAddress(
                    ClientStateViewProvider.Current.CurrentAvatarStateRaw.address,
                    recipeId) == state.Address)
            {
                HammerPointSubjectInternal.OnNext((recipeId, state));
            }
        }
    }
}

#endif
