using System.Collections.Generic;

namespace Nekoyume.SingleClient.Combat
{
    /// <summary>
    /// Bridges <see cref="SingleClientStageSheet"/>, the avatar's level-derived
    /// stats, and the pure <see cref="SingleClientBattle"/> simulator into a
    /// stage-play outcome.
    ///
    /// Pure function — no state mutation. The <see cref="SingleClientSession"/>
    /// layer is responsible for persisting clear-marking, exp grants, and
    /// action-point consumption based on the returned outcome.
    /// </summary>
    public static class SingleClientStageBattle
    {
        public static SingleClientStageBattleOutcome Execute(
            SingleClientStageRow stage,
            SingleClientAvatarState avatar,
            int seed = 0)
        {
            if (stage is null)
            {
                throw new System.ArgumentNullException(nameof(stage));
            }

            if (avatar is null)
            {
                throw new System.ArgumentNullException(nameof(avatar));
            }

            var attackerName = string.IsNullOrWhiteSpace(avatar.name)
                ? SingleClientState.DefaultAvatarName
                : avatar.name;

            var attacker = new SingleClientBattle.Combatant(
                attackerName,
                SingleClientAvatarStats.HpAtLevel(avatar.level),
                SingleClientAvatarStats.AtkAtLevel(avatar.level),
                SingleClientAvatarStats.DefAtLevel(avatar.level));

            var defender = new SingleClientBattle.Combatant(
                stage.DefenderName,
                stage.DefenderHp,
                stage.DefenderAtk,
                stage.DefenderDef);

            var result = SingleClientBattle.Simulate(attacker, defender, seed);
            var expGained = result.AttackerWins ? stage.ExpReward : 0L;

            return new SingleClientStageBattleOutcome(
                stage,
                attacker,
                defender,
                result,
                expGained);
        }
    }

    public sealed class SingleClientStageBattleOutcome
    {
        public SingleClientStageRow Stage { get; }
        public SingleClientBattle.Combatant Attacker { get; }
        public SingleClientBattle.Combatant Defender { get; }
        public SingleClientBattleResult Result { get; }
        public long ExpGained { get; }
        public bool PlayerWon => Result.AttackerWins;
        public int Turns => Result.Turns;
        public IReadOnlyList<string> Events => Result.Events;

        public SingleClientStageBattleOutcome(
            SingleClientStageRow stage,
            SingleClientBattle.Combatant attacker,
            SingleClientBattle.Combatant defender,
            SingleClientBattleResult result,
            long expGained)
        {
            Stage = stage;
            Attacker = attacker;
            Defender = defender;
            Result = result;
            ExpGained = expGained;
        }
    }
}
