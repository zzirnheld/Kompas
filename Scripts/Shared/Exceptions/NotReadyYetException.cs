using System;

namespace Kompas.Shared.Exceptions
{
	/// <summary>
    /// Indicates the value should have been initialized in Ready()
    /// </summary>
	public class NotReadyYetException
		: Exception
	{
	}
}