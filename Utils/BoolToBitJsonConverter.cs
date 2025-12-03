using System;
using Newtonsoft.Json;

namespace AsadorMoron.Utils
{
    public class BoolToBitJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(bool) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString().Equals("1", StringComparison.InvariantCultureIgnoreCase);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            int bitVal = Convert.ToBoolean(value) ? 1 : 0; writer.WriteValue(bitVal);
        }
    }
}
