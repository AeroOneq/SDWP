using System.Collections.Generic;

using ApplicationLib.Interfaces;
using Newtonsoft.Json;

namespace ApplicationLib.Models
{
    /// <summary>
    /// Class for storing data in the offline storages
    /// </summary>
    public class LocalDocumentation : ISerializable
    {
        public Documentation Documentation { get; set; }
        public List<Document> Documents { get; set; }
        public string DocumentationPath { get; set; }

        public LocalDocumentation(Documentation documentation, List<Document> documents)
        {
            Documentation = documentation;
            Documents = documents;
        }

        public static LocalDocumentation GetObjectFromJsonString(string jsonString)
        {
            return JsonConvert.DeserializeObject<LocalDocumentation>(jsonString);
        }

        public string GetJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
