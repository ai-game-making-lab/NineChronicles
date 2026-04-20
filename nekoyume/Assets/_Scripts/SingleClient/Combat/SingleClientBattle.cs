using System;
using System.Collections.Generic;

namespace Nekoyume.SingleClient.Combat
{
    /// <summary>
    /// Deterministic, Lib9c-free turn-based battle simulator. Replaces
    /// lib9c <c>StageSimulator</c>/<c>ArenaSimulator</c> for single-client mode.
    ///
    /// Inputs: attacker stats, defender stats, RNG seed (optional).
    /// Output: <see cref="SingleClientBattleResult"/> with winner flag +
    /// turn-by-turn log.
    ///
    /// Deliberately minimal — 1-vs-1, pure ATK/DEF/HP math. Skill/buff/elemental
    /// affinities are out of scope for this phase and can be added by subclassing
    /// <see cref="Apply"/> or composing the result.
    /// </summary>
    public static class SingleClientBattle
    {
        public readonly struct Combatant
        {
            public readonly string Name;
            public readonly long Hp;
            public readonly long Atk;
            public readonly long Def;

            public Combatant(string name, long hp, long atk, long def)
            {
                Name = name;
                Hp = Math.Max(1L, hp);
                Atk = Math.Max(0L, atk);
                Def = Math.Max(0L, def);
            }
        }

        public static SingleClientBattleResult Simulate(
            Combatant attacker,
            Combatant defender,
            int seed = 0,
            int maxTurns = 64)
        {
            var rng = new Random(seed == 0 ? Environment.TickCount : seed);
            var events = new List<string>(maxTurns * 2);
            var atkHp = attacker.Hp;
            var defHp = defender.Hp;
            var turn = 0;

            events.Add($"[T0] {attacker.Name}(HP:{atkHp}) vs {defender.Name}(HP:{defHp})");

            while (turn < maxTurns && atkHp > 0 && defHp > 0)
            {
                turn++;
                var atkDmg = ComputeDamage(attacker.Atk, defender.Def, rng);
                defHp -= atkDmg;
                events.Add($"[T{turn}.a] {attacker.Name} hits {defender.Name} for {atkDmg}  (defHP:{Math.Max(0, defHp)})");
                if (defHp <= 0)
                {
                    break;
                }

                var defDmg = ComputeDamage(defender.Atk, attacker.Def, rng);
                atkHp -= defDmg;
                events.Add($"[T{turn}.b] {defender.Name} hits {attacker.Name} for {defDmg}  (atkHP:{Math.Max(0, atkHp)})");
            }

            var attackerWins = atkHp > 0 && defHp <= 0;
            var defenderWins = defHp > 0 && atkHp <= 0;
            var winner = attackerWins ? attacker.Name : defenderWins ? defender.Name : "draw";
            events.Add($"[END] turns={turn}  winner={winner}  finalAtkHP={Math.Max(0, atkHp)}  finalDefHP={Math.Max(0, defHp)}");

            return new SingleClientBattleResult(
                attackerWins,
                defenderWins,
                turn,
                Math.Max(0, atkHp),
                Math.Max(0, defHp),
                events);
        }

        private static long ComputeDamage(long atk, long def, Random rng)
        {
            var raw = atk - (def / 2L);
            if (raw <= 0)
            {
                raw = 1L;
            }

            var jitter = rng.Next(80, 121);
            return Math.Max(1L, raw * jitter / 100L);
        }
    }

    public sealed class SingleClientBattleResult
    {
        public SingleClientBattleResult(
            bool attackerWins,
            bool defenderWins,
            int turns,
            long attackerRemainingHp,
            long defenderRemainingHp,
            IReadOnlyList<string> events)
        {
            AttackerWins = attackerWins;
            DefenderWins = defenderWins;
            Turns = turns;
            AttackerRemainingHp = attackerRemainingHp;
            DefenderRemainingHp = defenderRemainingHp;
            Events = events;
        }

        public bool AttackerWins { get; }
        public bool DefenderWins { get; }
        public bool IsDraw => !AttackerWins && !DefenderWins;
        public int Turns { get; }
        public long AttackerRemainingHp { get; }
        public long DefenderRemainingHp { get; }
        public IReadOnlyList<string> Events { get; }
    }
}
