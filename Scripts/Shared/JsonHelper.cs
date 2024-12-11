using System.IO;
using Newtonsoft.Json;

namespace Kompas.Shared;

public static class JsonHelper
{
	/*
	public static readonly ISerializationBinder KompasTypesBinder = ;

	private class KompasTypesBinder : ISerializationBinder
	{
		private ICollection<Type> allowedTypes = Assembly.GetExecutingAssembly().GetTypes()
			.Where(type => type.Namespace.Contains("Kompas"))
			.ToArray();

		public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
		{
			assemblyName = null;
			typeName = serializedType.Name;
		}

		public Type BindToType(string? assemblyName, string typeName)
		{
			return allowedTypes.Single(t => t.Name == typeName);
		}
	}*/ //TODO

	public static string PrettifyJson(string json)
	{
		using var stringReader = new StringReader(json);
		using var stringWriter = new StringWriter();

		var jsonReader = new JsonTextReader(stringReader);
		var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };

		jsonWriter.WriteToken(jsonReader);
		return stringWriter.ToString();
	}
}