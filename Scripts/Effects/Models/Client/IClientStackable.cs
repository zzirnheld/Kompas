using Godot;
using Kompas.Effects.Models;

namespace Kompas.Effects.Models.Client
{
	public interface IClientStackable : IStackable
	{
		/// <summary>
		/// The blurb for this stackable
		/// </summary>
		string StackableBlurb { get; }
	}
}