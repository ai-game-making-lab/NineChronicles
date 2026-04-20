namespace Nekoyume.SingleClient.Models.Quest
{
    /// Mirror of <c>Nekoyume.Model.Quest.QuestType</c> (ordinal-preserving).
    public enum QuestType
    {
        Adventure,
        Obtain,
        Craft,
        Exchange,
    }

    /// Mirror of lib9c's quest-reward category enum. Stubbed as a scaffold; lib9c does not expose
    /// a concrete <c>QuestRewardType</c> type today but UI specs reserve the name for future use.
    public enum QuestRewardType
    {
        Item,
        Currency,
    }
}
