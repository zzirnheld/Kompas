using System;
using Godot;

namespace Kompas.Shared.Exceptions;

public class UnassignedReferenceException
	: Exception
{
	private readonly string? fieldName;
	private readonly string? nodeName;

	public UnassignedReferenceException()
	{

	}

	public UnassignedReferenceException(string fieldName)
	{
		this.fieldName = fieldName;
	}

	public UnassignedReferenceException(string fieldName, string nodeName)
		: this(fieldName)
	{
		this.nodeName = nodeName;
	}

	public UnassignedReferenceException(string fieldName, Node node)
		: this(fieldName, node.Name)
	{}

	public override string ToString()
	{
		return $"{base.ToString()} {fieldName} on {nodeName}";
	}
}