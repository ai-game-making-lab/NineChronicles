using Lib9cBuffLimitSheet = Nekoyume.TableData.BuffLimitSheet;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Projects a lib9c <see cref="Lib9cBuffLimitSheet.Row"/> into the client-owned
    /// <see cref="BuffLimitRowView"/>. The lib9c row carries only two scalar fields
    /// (<c>Value</c>) alongside the group id; this slice treats that single value as the
    /// duration/stack cap the tooltip renders via
    /// <c>SKILL_DESCRIPTION_STATDEBUFF_LIMIT</c>. If future csv revisions split the cap into
    /// distinct duration-vs-stack columns, <see cref="BuffLimitRowView.DurationStack"/> is
    /// already reserved.
    /// </summary>
    public static class BuffLimitRowMapper
    {
        public static BuffLimitRowView ToView(this Lib9cBuffLimitSheet.Row source)
        {
            if (source is null)
            {
                return default;
            }

            return new BuffLimitRowView(
                groupId: source.GroupId,
                durationLimit: source.Value,
                durationStack: source.Value);
        }
    }
}
