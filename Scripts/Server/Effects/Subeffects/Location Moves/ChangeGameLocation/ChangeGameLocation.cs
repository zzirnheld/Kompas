using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	/// <summary>
	/// Moves cards between discard/field/etc
	/// </summary>
	public abstract class ChangeGameLocation : ServerSubeffect
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
		{
			var currLocation = GetCardTarget(overrideContext)?.LocationModel;
			return currLocation == null || currLocation == Destination;
		}

		protected abstract ILocationModel Destination { get; }

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null) throw new NullCardException(TargetWasNull);

			ChangeLocation(CardTarget);
			return Task.FromResult(ResolutionInfo.Next);
		}

		protected abstract void ChangeLocation(GameCard card);
	}
}