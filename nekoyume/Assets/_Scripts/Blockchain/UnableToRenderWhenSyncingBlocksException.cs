#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Runtime.Serialization;

namespace Nekoyume.Blockchain
{
    [Serializable]
    public class UnableToRenderWhenSyncingBlocksException : Exception
    {
        public UnableToRenderWhenSyncingBlocksException() : base()
        {
        }

        public UnableToRenderWhenSyncingBlocksException(string message) : base(message)
        {
        }

        public UnableToRenderWhenSyncingBlocksException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}

#endif
