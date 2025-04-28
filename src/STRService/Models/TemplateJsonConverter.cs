using System.Text.Json;
using System.Text.Json.Serialization;

using ARCtrl;
using STRIndex;

namespace STRService.Models
{
    public class TemplateJsonConverter : JsonConverter<Template>
    {
        public override Template Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = reader.GetString();

            if (string.IsNullOrEmpty(json))
            {
                throw new JsonException("The encoded Template string is null or empty.");
            }

            // This assumes you expose the F# method as a .NET-accessible static method
            return Wrapper.templateFromJson(json);
        }

        public override void Write(Utf8JsonWriter writer, Template value, JsonSerializerOptions options)
        {
            var encoded = Wrapper.templateToJson(value);
            writer.WriteStringValue(encoded);
        }
    }
}
