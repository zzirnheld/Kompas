using System;

namespace Kompas.Shared.Exceptions
{
	/// <summary>
    /// Indicates the value should have been initialized already.
	/// For something that should be initialized in Ready(), use <see cref="NotReadyYetException"/>
    /// </summary>
	public class NotInitializedException
		: Exception
	{
	}
}