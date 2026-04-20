#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.SingleClient.State;
using System;
using Libplanet.Types.Assets;
using Nekoyume.SingleClient;
using UniRx;

namespace Nekoyume.State.Subjects
{
    /// <summary>
    /// The change of the value included in `AgentState` is notified to the outside through each Subject
    /// </summary>
    public static class AgentStateSubject
    {
        private static readonly Subject<FungibleAssetValue> GoldInternal;
        private static readonly Subject<FungibleAssetValue> CrystalInternal;
        private static readonly Subject<FungibleAssetValue> GarageInternal;

        public static readonly IObservable<FungibleAssetValue> Gold;
        public static readonly IObservable<FungibleAssetValue> Crystal;
        public static readonly IObservable<FungibleAssetValue> Garage;

        static AgentStateSubject()
        {
            GoldInternal = new Subject<FungibleAssetValue>();
            CrystalInternal = new Subject<FungibleAssetValue>();
            GarageInternal = new Subject<FungibleAssetValue>();
            Gold = GoldInternal.ObserveOnMainThread();
            Crystal = CrystalInternal.ObserveOnMainThread();
            Garage = GarageInternal.ObserveOnMainThread();
        }

        public static void OnNextGold(FungibleAssetValue gold)
        {
            if (gold.Currency.Equals(ClientStateViewProvider.Current.CurrentGoldBalanceStateRaw.Gold.Currency))
            {
                GoldInternal.OnNext(gold);
            }
        }

        public static void OnNextCrystal(FungibleAssetValue crystal)
        {
            if (crystal.Currency.Equals(ClientCurrencies.Crystal))
            {
                CrystalInternal.OnNext(crystal);
            }
        }

        public static void OnNextGarage(FungibleAssetValue garage)
        {
            if (garage.Currency.Equals(ClientCurrencies.Garage))
            {
                GarageInternal.OnNext(garage);
            }
        }
    }
}

#endif
