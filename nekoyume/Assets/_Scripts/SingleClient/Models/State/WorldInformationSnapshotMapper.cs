using System.Collections.Generic;
using Bencodex.Types;
using Lib9cWorld = Nekoyume.Model.WorldInformation.World;
using Lib9cWorldInformation = Nekoyume.Model.WorldInformation;

namespace Nekoyume.SingleClient.Models.State
{
    /// <summary>
    /// Projects lib9c's <see cref="Lib9cWorldInformation"/> into a
    /// <see cref="WorldInformationSnapshot"/>. lib9c's public surface only exposes
    /// <c>TryGetWorld(id, out)</c> / <c>TryGetLastClearedStageId(out)</c> — there is no
    /// public enumerator — so this mapper reads the bencodex dictionary returned by
    /// <c>Serialize()</c> to walk every <c>World</c> entry once at mapping time.
    /// </summary>
    /// <remarks>
    /// Round-tripping through <c>Serialize()</c> is the only public-API-safe way to enumerate
    /// all worlds without reflecting into the private <c>_worlds</c> field. The cost is one
    /// extra allocation per avatar state sync — acceptable because <see cref="ToView"/> is
    /// called from the state-sync path (once per block tick), not per-frame.
    /// </remarks>
    public static class WorldInformationSnapshotMapper
    {
        public static WorldInformationSnapshot ToView(this Lib9cWorldInformation source)
        {
            if (source is null)
            {
                return default;
            }

            var worlds = EnumerateWorlds(source);

            // Mirror WorldInformation.TryGetLastClearedStageId exactly: default(int) = 0 when
            // nothing is cleared. We cannot call the lib9c method directly via the snapshot
            // list because Mimisbrunnr filtering needs GameConfig.MimisbrunnrWorldId, which the
            // lib9c helper already handles. Delegate to the source to stay in lockstep with
            // lib9c semantics even when the Mimisbrunnr id changes upstream.
            //
            // Guard for the null-sheet test fixture: a WorldInformation constructed from a
            // null WorldSheet has null internals and throws on any lookup. In that case
            // "nothing is cleared" is the correct answer anyway.
            int lastStageIdCleared;
            try
            {
                lastStageIdCleared = source.TryGetLastClearedStageId(out var stageId) ? stageId : 0;
            }
            catch (System.NullReferenceException)
            {
                lastStageIdCleared = 0;
            }

            return new WorldInformationSnapshot(worlds, lastStageIdCleared);
        }

        private static IReadOnlyList<WorldSnapshot> EnumerateWorlds(Lib9cWorldInformation source)
        {
            // Serialize() always returns a Dictionary whose keys are the world ids as
            // bencodex Text values; the values are the per-world serialized dictionaries
            // accepted by World's ctor. When the source was built with a null WorldSheet
            // (e.g., test fixtures), Serialize() throws NullReferenceException — guard that
            // by probing Serialize() under a try/catch rather than trying to peek at private
            // fields.
            Dictionary serialized;
            try
            {
                serialized = source.Serialize() as Dictionary;
            }
            catch (System.NullReferenceException)
            {
                return System.Array.Empty<WorldSnapshot>();
            }

            if (serialized is null)
            {
                return System.Array.Empty<WorldSnapshot>();
            }

            var list = new List<WorldSnapshot>(serialized.Count);
            foreach (var kv in serialized)
            {
                if (kv.Value is not Dictionary worldDict)
                {
                    continue;
                }

                var world = new Lib9cWorld(worldDict);
                list.Add(world.ToView());
            }

            // World ids are not inherently ordered by the bencodex dictionary iteration, so sort
            // by id to give the UI a stable render order.
            list.Sort((a, b) => a.Id.CompareTo(b.Id));
            return list;
        }
    }
}
