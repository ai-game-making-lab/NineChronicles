#if LIB9C_RESTORED // stubbed out after lib9c deletion
namespace Nekoyume.SingleClient
{
    public interface ISingleClientStateStore
    {
        bool Exists { get; }

        SingleClientState LoadOrCreate();

        void Save(SingleClientState state);
    }
}

#endif
