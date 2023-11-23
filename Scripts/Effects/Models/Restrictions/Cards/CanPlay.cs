using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class CanPlay : CardRestrictionBase
	{
		[JsonProperty]
		public IIdentity<Space> destination;
		[JsonProperty]
		public IIdentity<IPlayer> player = new Identities.Players.TargetIndex();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			destination?.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
		{
			var controller = player.From(context);
			bool IsValidEffectPlay(Space space) => card.PlayRestriction.IsValid((space, controller), context);

			if (destination == null) return Space.Spaces.Any(IsValidEffectPlay);
			else return IsValidEffectPlay(destination.From(context));
		}
	}
}