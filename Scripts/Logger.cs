using Godot;

namespace Kompas
{
	//Exists for unit testing to be able to override calls to GD.Print, which otherwise segfaults XUnit.
	public interface IKompasLogger
	{
		void Log(object? s);
		void Warn(object? s);
		void Err(object? s);
	}

    internal class KompasLogger : IKompasLogger
    {
		void IKompasLogger.Err(object? s) => GD.PushError(s);

		void IKompasLogger.Log(object? s) => GD.Print(s);

		void IKompasLogger.Warn(object? s) => GD.PushWarning(s);
    }

	/// <summary>
	/// Usage: use the Log, Warn, and Err functions on this static class
	/// </summary>
	public class Logger
	{
		private static Logger? _singleton;
		public static Logger Singleton => _singleton ??= new Logger();

		/// <summary>
		/// Public set, so that the logger can be replaced for unit testing.
		/// Private get, so that you're forced to go through the static functions.
		/// </summary>
		public IKompasLogger KompasLogger { private get; set; } = new KompasLogger();

		public static void Log(object s) => Singleton.KompasLogger.Log(s);
		public static void Warn(object s) => Singleton.KompasLogger.Warn(s);
		public static void Err(object s) => Singleton.KompasLogger.Err(s);
	}
}