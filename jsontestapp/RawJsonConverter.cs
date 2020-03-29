using Newtonsoft.Json;
using System;

namespace jsontestapp
{
    public class RawJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override async void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            await writer.WriteRawValueAsync((string)value);
        }
    }
}
