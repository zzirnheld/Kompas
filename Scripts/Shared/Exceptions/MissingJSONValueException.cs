namespace Kompas.Shared.Exceptions;

[System.Serializable]
public class MissingJSONValueException : System.Exception
{
	public MissingJSONValueException() { }
	public MissingJSONValueException(string message) : base(message) { }
	public MissingJSONValueException(string message, System.Exception inner) : base(message, inner) { }
	public MissingJSONValueException(string fieldName, object o) : base($"{o} was missing {fieldName}") { }
	protected MissingJSONValueException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}