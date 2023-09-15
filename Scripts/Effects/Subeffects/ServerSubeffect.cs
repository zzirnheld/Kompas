using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Subeffects
{
	/// <summary>
	/// Not abstract because it's instantiated as part of loading subeffects
	/// </summary>
	public class ServerSubeffect : Subeffect
	{
		public bool IsImpossible() { throw new System.NotImplementedException(); }
	}
}