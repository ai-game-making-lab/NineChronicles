namespace Nekoyume.SingleClient
{
    public interface ISingleClientStateStore
    {
        bool Exists { get; }

        SingleClientState LoadOrCreate();

        void Save(SingleClientState state);
    }
}
