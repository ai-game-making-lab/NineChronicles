using Lib9cElementalResult = Nekoyume.Model.Elemental.ElementalResult;
using Lib9cElementalType = Nekoyume.Model.Elemental.ElementalType;
using SingleClientElementalResult = Nekoyume.SingleClient.Models.Elemental.ElementalResult;
using SingleClientElementalType = Nekoyume.SingleClient.Models.Elemental.ElementalType;

namespace Nekoyume.SingleClient.Models.Elemental
{
    /// <summary>
    /// Ordinal-compatible casts between lib9c <c>ElementalType</c>/<c>ElementalResult</c> and
    /// their client-owned mirrors. Kept as explicit helpers so call sites document the boundary
    /// crossing instead of hiding it behind implicit casts.
    /// </summary>
    public static class ElementalTypeMapper
    {
        public static SingleClientElementalType ToView(this Lib9cElementalType source)
        {
            return (SingleClientElementalType)(int)source;
        }

        public static Lib9cElementalType ToLib9c(this SingleClientElementalType source)
        {
            return (Lib9cElementalType)(int)source;
        }

        public static SingleClientElementalResult ToView(this Lib9cElementalResult source)
        {
            return (SingleClientElementalResult)(int)source;
        }

        public static Lib9cElementalResult ToLib9c(this SingleClientElementalResult source)
        {
            return (Lib9cElementalResult)(int)source;
        }
    }
}
