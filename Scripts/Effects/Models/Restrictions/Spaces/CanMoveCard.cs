using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;
using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	/// <summary>
	/// Whether a card can be moved to that space. Presumes from effect
	/// </summary>
	public class CanMoveCard : SpaceRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCardInfo> toMove;

		/// <summary>
		/// Describes any restriction on the spaces between the card and where it needs to go (the space being tested)
		/// </summary>
		[JsonProperty]
		public IRestriction<Space> throughRestriction = new Empty();

		[JsonProperty]
		public IRestriction<int> distanceRestriction = new Gamestate.AlwaysValid();

		[JsonProperty]
		public bool normalMove = false;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			toMove.Initialize(initializationContext);
			throughRestriction?.Initialize(initializationContext);
			distanceRestriction?.Initialize(initializationContext);
		}

		private bool FitsMovementRestriction(IGameCardInfo card, Space space, IResolutionContext context)
			=> normalMove 
				? card.MovementRestriction.IsValid(space, ResolutionContext.PlayerTrigger(InitializationContext.effect, InitializationContext.game))
				: card.MovementRestriction.IsValid(space, context);

		private bool FitsThroughRestriction(Space source, Space dest, IResolutionContext context)
			=> Kompas.Gamestate.Locations.Models.Board.AreConnectedByNumberOfSpacesFittingPredicate(source, dest,
				s => throughRestriction.IsValid(s, context), d => distanceRestriction.IsValid(d, context));

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			var card = toMove.From(context).Card;
			return FitsMovementRestriction(card, space, context) && FitsThroughRestriction(card.Position, space, context);
		}

	}
}