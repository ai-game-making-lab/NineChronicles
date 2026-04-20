#if LIB9C_RESTORED // stubbed out after lib9c deletion
using Nekoyume.Model.State;

namespace Nekoyume.State.Modifiers
{
    public abstract class CombinationSlotStateModifier : IStateModifier<CombinationSlotState>
    {
        public bool dirty { get; set; }
        public abstract bool IsEmpty { get; }
        public abstract CombinationSlotState Modify(CombinationSlotState state);
    }
}

#endif
