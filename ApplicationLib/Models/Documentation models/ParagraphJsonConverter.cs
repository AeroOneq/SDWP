using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ApplicationLib.Models
{
    class ParagraphJsonConverter : CustomCreationConverter<Paragraph>
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var paragraph = new Paragraph();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                    return paragraph;

                if (reader.TokenType != JsonToken.PropertyName)
                    continue;

                string propertyName = reader.Value.ToString();

                if (!reader.Read())
                    return paragraph;

                if ("type".Equals(propertyName))
                {
                    paragraph.Type = reader.Value?.ToString();
                }
                else if ("element".Equals(propertyName))
                {
                    switch (paragraph.Type)
                    {
                        case "Subparagraph":
                            paragraph.ParagraphElement = serializer.Deserialize<Subparagraph>(reader);
                            break;
                        case "Table":
                            paragraph.ParagraphElement = serializer.Deserialize<Table>(reader);
                            break;
                        case "NumberedList":
                            paragraph.ParagraphElement = serializer.Deserialize<NumberedList>(reader);
                            break;
                        case "ParagraphImage":
                            paragraph.ParagraphElement = serializer.Deserialize<ParagraphImage>(reader);
                            break;
                    }
                }
            }

            return paragraph;
        }

        public override Paragraph Create(Type objectType)
        {
            return new Paragraph();
        }
    }
}
