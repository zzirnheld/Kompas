using Godot;
using Kompas.Effects.Models;

namespace Kompas.Client.Effects.Models
{
	public interface IClientStackable : IStackable
	{
		/// <summary>
		/// The blurb for this stackable
		/// </summary>
		string StackableBlurb { get; }
	}
}