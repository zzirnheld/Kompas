using System.IO;
using Newtonsoft.Json;

namespace Kompas.Shared
{
	public static class JsonHelper
	{
		public static string PrettifyJson(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }
	}
}