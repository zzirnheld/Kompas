using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;

namespace Kompas.Effects.Subeffects
{
	public class SpaceTarget : Subeffect
	{
		#nullable disable
		//TODO, once subeffects get copied in
		public IRestriction<Space> spaceRestriction;
		#nullable restore

		public bool WillBePossibleIfCardTargeted(GameCard? theoreticalTarget) => throw new System.NotImplementedException();
	}	
}