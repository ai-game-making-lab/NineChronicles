#if LIB9C_RESTORED // stubbed out after lib9c deletion
﻿using System;
using System.Text.Json.Serialization;

namespace Nekoyume.UI
{
    [Serializable]
    public class TutorialPreset
    {
        [JsonInclude]
        public Preset[] preset;
    }

    [Serializable]
    public class Preset : IEquatable<Preset>
    {
        [JsonInclude]
        public int id;

        [JsonInclude]
        public string content;

        [JsonInclude]
        public bool isExistFadeInBackground;

        [JsonInclude]
        public bool isEnableMask;

        [JsonInclude]
        public bool isSkipArrowAnimation;

        [JsonInclude]
        public int commaId;

        public bool Equals(Preset other)
        {
            return other is not null &&
                id == other.id &&
                content == other.content &&
                isExistFadeInBackground == other.isExistFadeInBackground &&
                isEnableMask == other.isEnableMask &&
                isSkipArrowAnimation == other.isSkipArrowAnimation &&
                commaId == other.commaId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Preset);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + id;
                hash = hash * 31 + (content?.GetHashCode() ?? 0);
                hash = hash * 31 + isExistFadeInBackground.GetHashCode();
                hash = hash * 31 + isEnableMask.GetHashCode();
                hash = hash * 31 + isSkipArrowAnimation.GetHashCode();
                hash = hash * 31 + commaId;
                return hash;
            }
        }
    }
}

#endif
