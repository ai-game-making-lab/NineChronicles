#if LIB9C_RESTORED // stubbed out after lib9c deletion
using System.Text.Json.Serialization;

namespace Nekoyume.GraphQL.GraphTypes
{
    public class StateQueryGraphType<T>
    {
        [JsonPropertyName("stateQuery")]
        public T StateQuery;
    }
}

#endif
