#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Libplanet.Action;

namespace Tests.EditMode
{
    public class Random : System.Random, IRandom
    {
        public Random(int seed = int.MaxValue) : base(seed)
        {
        }

        public int Seed => throw new System.NotImplementedException();
    }
}

#endif
