using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HomeRecipes.Shared
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, [AllowNull] TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
            => TimeSpan.Parse(reader.ReadAsString());

        public override void WriteJson(JsonWriter writer, [AllowNull] TimeSpan value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString("hh:mm:ss"));
    }
}
