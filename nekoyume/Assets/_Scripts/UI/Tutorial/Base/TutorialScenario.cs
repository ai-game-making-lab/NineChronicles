#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Nekoyume.UI
{
    [Serializable]
    public class TutorialScenario
    {
        [JsonInclude]
        public Scenario[] scenario;
    }

    [Serializable]
    public class Scenario : IEquatable<Scenario>
    {
        [JsonInclude]
        public int id;

        [JsonInclude]
        public int nextId;

        [JsonInclude]
        public int checkPointId;

        [JsonInclude]
        public ScenarioData data;

        public bool Equals(Scenario other)
        {
            return other is not null &&
                id == other.id &&
                nextId == other.nextId &&
                checkPointId == other.checkPointId &&
                Equals(data, other.data);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Scenario);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + id;
                hash = hash * 31 + nextId;
                hash = hash * 31 + checkPointId;
                hash = hash * 31 + (data?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }

    [Serializable]
    public class ScenarioData : IEquatable<ScenarioData>
    {
        [JsonInclude]
        public int presetId;

        [JsonInclude]
        [JsonConverter(typeof(Vector2JsonConverter))]
        public Vector2 arrowPositionOffset;

        [JsonInclude]
        [JsonConverter(typeof(Vector2JsonConverter))]
        public Vector2 targetPositionOffset;

        [JsonInclude]
        [JsonConverter(typeof(Vector2JsonConverter))]
        public Vector2 targetSizeOffset;

        [JsonInclude]
        [JsonConverter(typeof(Vector4JsonConverter))]
        public Vector4 buttonRaycastPadding;

        [JsonInclude]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public TutorialTargetType targetType;

        [JsonInclude]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public TutorialActionType actionType;

        [JsonInclude]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public GuideType guideType;

        [JsonInclude]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public DialogEmojiType emojiType;

        [JsonInclude]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public DialogPositionType dialogPositionType;

        [JsonInclude]
        public string scriptKey;

        [JsonInclude]
        public float arrowAdditionalDelay;

        [JsonInclude]
        public bool fullScreenButton;

        [JsonInclude]
        public bool noArrow;

        [JsonInclude]
        public Sprite guideSprite;

        public bool Equals(ScenarioData other)
        {
            return other is not null &&
                presetId == other.presetId &&
                arrowPositionOffset.Equals(other.arrowPositionOffset) &&
                targetPositionOffset.Equals(other.targetPositionOffset) &&
                targetSizeOffset.Equals(other.targetSizeOffset) &&
                buttonRaycastPadding.Equals(other.buttonRaycastPadding) &&
                targetType == other.targetType &&
                actionType == other.actionType &&
                guideType == other.guideType &&
                emojiType == other.emojiType &&
                dialogPositionType == other.dialogPositionType &&
                scriptKey == other.scriptKey &&
                arrowAdditionalDelay.Equals(other.arrowAdditionalDelay) &&
                fullScreenButton == other.fullScreenButton &&
                noArrow == other.noArrow &&
                Equals(guideSprite, other.guideSprite);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ScenarioData);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + presetId;
                hash = hash * 31 + arrowPositionOffset.GetHashCode();
                hash = hash * 31 + targetPositionOffset.GetHashCode();
                hash = hash * 31 + targetSizeOffset.GetHashCode();
                hash = hash * 31 + buttonRaycastPadding.GetHashCode();
                hash = hash * 31 + targetType.GetHashCode();
                hash = hash * 31 + actionType.GetHashCode();
                hash = hash * 31 + guideType.GetHashCode();
                hash = hash * 31 + emojiType.GetHashCode();
                hash = hash * 31 + dialogPositionType.GetHashCode();
                hash = hash * 31 + (scriptKey?.GetHashCode() ?? 0);
                hash = hash * 31 + arrowAdditionalDelay.GetHashCode();
                hash = hash * 31 + fullScreenButton.GetHashCode();
                hash = hash * 31 + noArrow.GetHashCode();
                hash = hash * 31 + (guideSprite?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }

    public sealed class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var x = 0f;
            var y = 0f;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new Vector2(x, y);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                switch (propertyName)
                {
                    case "x":
                        x = UnityJsonConverterHelper.ReadSingle(ref reader);
                        break;
                    case "y":
                        y = UnityJsonConverterHelper.ReadSingle(ref reader);
                        break;
                    default:
                        UnityJsonConverterHelper.SkipValue(ref reader);
                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector2 value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.x);
            writer.WriteNumber("y", value.y);
            writer.WriteEndObject();
        }
    }

    public sealed class Vector4JsonConverter : JsonConverter<Vector4>
    {
        public override Vector4 Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var x = 0f;
            var y = 0f;
            var z = 0f;
            var w = 0f;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new Vector4(x, y, z, w);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                if (!reader.Read())
                {
                    throw new JsonException();
                }

                switch (propertyName)
                {
                    case "x":
                        x = UnityJsonConverterHelper.ReadSingle(ref reader);
                        break;
                    case "y":
                        y = UnityJsonConverterHelper.ReadSingle(ref reader);
                        break;
                    case "z":
                        z = UnityJsonConverterHelper.ReadSingle(ref reader);
                        break;
                    case "w":
                        w = UnityJsonConverterHelper.ReadSingle(ref reader);
                        break;
                    default:
                        UnityJsonConverterHelper.SkipValue(ref reader);
                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            Vector4 value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("x", value.x);
            writer.WriteNumber("y", value.y);
            writer.WriteNumber("z", value.z);
            writer.WriteNumber("w", value.w);
            writer.WriteEndObject();
        }
    }

    internal static class UnityJsonConverterHelper
    {
        public static float ReadSingle(ref Utf8JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => (float)reader.GetDouble(),
                JsonTokenType.String when float.TryParse(reader.GetString(), out var value) => value,
                _ => throw new JsonException(),
            };
        }

        public static void SkipValue(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject &&
                reader.TokenType != JsonTokenType.StartArray)
            {
                return;
            }

            var depth = 0;
            do
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                    case JsonTokenType.StartArray:
                        depth++;
                        break;
                    case JsonTokenType.EndObject:
                    case JsonTokenType.EndArray:
                        depth--;
                        break;
                }
            } while (depth > 0 && reader.Read());
        }
    }
}

#endif
