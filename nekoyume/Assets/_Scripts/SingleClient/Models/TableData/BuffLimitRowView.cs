#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;

namespace Nekoyume.SingleClient.Models.TableData
{
    /// <summary>
    /// Immutable client-owned mirror of <c>Nekoyume.TableData.BuffLimitSheet.Row</c>. The UI
    /// reads two values: the group this limit applies to and the magnitude cap. The lib9c
    /// <c>StatModifier</c> OperationType is deliberately omitted here because the tooltip only
    /// formats the scalar cap; when stack-count rendering is added in a later slice it will
    /// layer on top of <see cref="DurationStack"/>.
    /// </summary>
    public readonly struct BuffLimitRowView : IEquatable<BuffLimitRowView>
    {
        public int GroupId { get; }
        public int DurationLimit { get; }
        public int DurationStack { get; }

        public BuffLimitRowView(int groupId, int durationLimit, int durationStack)
        {
            GroupId = groupId;
            DurationLimit = durationLimit;
            DurationStack = durationStack;
        }

        public bool Equals(BuffLimitRowView other) => GroupId == other.GroupId;

        public override bool Equals(object obj) => obj is BuffLimitRowView other && Equals(other);

        public override int GetHashCode() => GroupId;

        public static bool operator ==(BuffLimitRowView left, BuffLimitRowView right) => left.Equals(right);

        public static bool operator !=(BuffLimitRowView left, BuffLimitRowView right) => !left.Equals(right);
    }
}

#endif
