#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Linq;
using NUnit.Framework;
using Nekoyume.SingleClient.Models.EnumType;

// Lib9c source enums (read-only; do not modify).
using Lib9cArenaType = Nekoyume.Model.EnumType.ArenaType;
using Lib9cBattleType = Nekoyume.Model.EnumType.BattleType;
using Lib9cCraftType = Nekoyume.Model.EnumType.CraftType;
using Lib9cGrade = Nekoyume.Model.EnumType.Grade;
using Lib9cRuneSlotType = Nekoyume.Model.EnumType.RuneSlotType;
using Lib9cRuneType = Nekoyume.Model.EnumType.RuneType;
using Lib9cRuneUsePlace = Nekoyume.Model.EnumType.RuneUsePlace;
using Lib9cStatReferenceType = Nekoyume.Model.EnumType.StatReferenceType;
using Lib9cTradeType = Nekoyume.Model.EnumType.TradeType;

// Client-owned mirrors.
using ArenaType = Nekoyume.SingleClient.Models.EnumType.ArenaType;
using BattleType = Nekoyume.SingleClient.Models.EnumType.BattleType;
using CraftType = Nekoyume.SingleClient.Models.EnumType.CraftType;
using Grade = Nekoyume.SingleClient.Models.EnumType.Grade;
using RuneSlotType = Nekoyume.SingleClient.Models.EnumType.RuneSlotType;
using RuneType = Nekoyume.SingleClient.Models.EnumType.RuneType;
using RuneUsePlace = Nekoyume.SingleClient.Models.EnumType.RuneUsePlace;
using StatReferenceType = Nekoyume.SingleClient.Models.EnumType.StatReferenceType;
using TradeType = Nekoyume.SingleClient.Models.EnumType.TradeType;

namespace Tests.EditMode.SingleClient.EnumType
{
    /// <summary>
    /// Round-trip + name-parity coverage for every <c>Nekoyume.Model.EnumType.*</c> mirror.
    /// Round-trip guards ordinal drift; name-parity guards stealth lib9c re-ordering or renames.
    /// </summary>
    public class EnumTypeMapperTest
    {
        // ---------- ArenaType ----------

        [Test]
        public void ArenaTypeRoundTrip()
        {
            foreach (Lib9cArenaType v in Enum.GetValues(typeof(Lib9cArenaType)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"ArenaType ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"ArenaType round-trip failed for {v}.");
            }
        }

        [Test]
        public void ArenaTypeNameParity()
        {
            AssertNameParity(typeof(Lib9cArenaType), typeof(ArenaType));
        }

        // ---------- BattleType ----------

        [Test]
        public void BattleTypeRoundTrip()
        {
            foreach (Lib9cBattleType v in Enum.GetValues(typeof(Lib9cBattleType)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"BattleType ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"BattleType round-trip failed for {v}.");
            }
        }

        [Test]
        public void BattleTypeNameParity()
        {
            AssertNameParity(typeof(Lib9cBattleType), typeof(BattleType));
        }

        // ---------- CraftType ----------

        [Test]
        public void CraftTypeRoundTrip()
        {
            foreach (Lib9cCraftType v in Enum.GetValues(typeof(Lib9cCraftType)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"CraftType ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"CraftType round-trip failed for {v}.");
            }
        }

        [Test]
        public void CraftTypeNameParity()
        {
            AssertNameParity(typeof(Lib9cCraftType), typeof(CraftType));
        }

        // ---------- Grade ----------

        [Test]
        public void GradeRoundTrip()
        {
            foreach (Lib9cGrade v in Enum.GetValues(typeof(Lib9cGrade)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"Grade ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"Grade round-trip failed for {v}.");
            }
        }

        [Test]
        public void GradeNameParity()
        {
            AssertNameParity(typeof(Lib9cGrade), typeof(Grade));
        }

        // ---------- RuneSlotType ----------

        [Test]
        public void RuneSlotTypeRoundTrip()
        {
            foreach (Lib9cRuneSlotType v in Enum.GetValues(typeof(Lib9cRuneSlotType)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"RuneSlotType ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"RuneSlotType round-trip failed for {v}.");
            }
        }

        [Test]
        public void RuneSlotTypeNameParity()
        {
            AssertNameParity(typeof(Lib9cRuneSlotType), typeof(RuneSlotType));
        }

        // ---------- RuneType ----------

        [Test]
        public void RuneTypeRoundTrip()
        {
            foreach (Lib9cRuneType v in Enum.GetValues(typeof(Lib9cRuneType)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"RuneType ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"RuneType round-trip failed for {v}.");
            }
        }

        [Test]
        public void RuneTypeNameParity()
        {
            AssertNameParity(typeof(Lib9cRuneType), typeof(RuneType));
        }

        // ---------- RuneUsePlace ----------

        [Test]
        public void RuneUsePlaceRoundTrip()
        {
            foreach (Lib9cRuneUsePlace v in Enum.GetValues(typeof(Lib9cRuneUsePlace)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"RuneUsePlace ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"RuneUsePlace round-trip failed for {v}.");
            }
        }

        [Test]
        public void RuneUsePlaceNameParity()
        {
            AssertNameParity(typeof(Lib9cRuneUsePlace), typeof(RuneUsePlace));
        }

        // ---------- StatReferenceType ----------

        [Test]
        public void StatReferenceTypeRoundTrip()
        {
            foreach (Lib9cStatReferenceType v in Enum.GetValues(typeof(Lib9cStatReferenceType)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"StatReferenceType ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"StatReferenceType round-trip failed for {v}.");
            }
        }

        [Test]
        public void StatReferenceTypeNameParity()
        {
            AssertNameParity(typeof(Lib9cStatReferenceType), typeof(StatReferenceType));
        }

        // ---------- TradeType ----------

        [Test]
        public void TradeTypeRoundTrip()
        {
            foreach (Lib9cTradeType v in Enum.GetValues(typeof(Lib9cTradeType)))
            {
                Assert.AreEqual((int)v, (int)v.ToView(),
                    $"TradeType ordinal mismatch for {v}.");
                Assert.AreEqual(v, v.ToView().ToLib9c(),
                    $"TradeType round-trip failed for {v}.");
            }
        }

        [Test]
        public void TradeTypeNameParity()
        {
            AssertNameParity(typeof(Lib9cTradeType), typeof(TradeType));
        }

        // ---------- helpers ----------

        private static void AssertNameParity(Type lib9c, Type mirror)
        {
            var lib9cNames = Enum.GetNames(lib9c).OrderBy(n => n).ToArray();
            var mirrorNames = Enum.GetNames(mirror).OrderBy(n => n).ToArray();
            CollectionAssert.AreEqual(lib9cNames, mirrorNames,
                $"Enum name set drift detected between lib9c {lib9c.FullName} and mirror " +
                $"{mirror.FullName}. If lib9c intentionally added/renamed a member, update " +
                $"the mirror in the same slice.");
        }
    }
}

#endif
