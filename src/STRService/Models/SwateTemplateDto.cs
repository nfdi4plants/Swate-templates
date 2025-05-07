
using System.Text.Json;
using System.Text.Json.Serialization;

using ARCtrl;
using ARCtrl.Contract;

namespace STRService.Models
{
    public class SwateTemplateDto
    {
        public static SwateTemplateDto Create(
            SwateTemplate content,
            SwateTemplateMetadata metadata) =>
                new SwateTemplateDto
                {
                    Content = content,
                    Metadata = metadata
                };

        public required SwateTemplate Content { get; set; }
        public required SwateTemplateMetadata Metadata { get; set; }

        public static SwateTemplateDto FromJsonString(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            // If TemplateJsonConverter isn't applied via attribute, you could add it here instead:
            // options.Converters.Add(new TemplateJsonConverter());

            return JsonSerializer.Deserialize<SwateTemplateDto>(json, options)
                ?? throw new JsonException("Failed to deserialize SwateTemplateDto.");
        }

        public string ToJsonString()
        {
            return SwateTemplateDto.ToJsonString(this);
        }

        public static string ToJsonString(SwateTemplateDto swateTemplateDto)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(swateTemplateDto, options);
        }
    }
}
