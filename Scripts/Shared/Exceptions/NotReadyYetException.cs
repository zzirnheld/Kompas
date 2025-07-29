using System;
using Godot;

namespace Kompas.Shared.Exceptions;

/// <summary>
/// Indicates the value should have been initialized in Ready()
/// </summary>
public class NotReadyYetException
	: Exception
{
	private readonly string? fieldName;
	private readonly string? nodeName;

	public NotReadyYetException()
	{

	}

	public NotReadyYetException(string fieldName)
	{
		this.fieldName = fieldName;
	}

	public NotReadyYetException(string fieldName, string nodeName)
		: this(fieldName)
	{
		this.nodeName = nodeName;
	}

	public NotReadyYetException(string fieldName, Node node)
		: this(fieldName, node.Name)
	{}

	public override string ToString()
	{
		return $"{base.ToString()} {fieldName} on {nodeName}";
	}
}