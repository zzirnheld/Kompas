using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions.Triggering;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	/// <summary>
	/// Base class for trigger restrictions that can also act like gamestate restrictions
	/// </summary>
	public abstract class TriggerGamestateRestrictionBase : TriggerRestrictionBase, IGamestateRestriction
	{
		public bool IsValid(IResolutionContext context) => IsValid(context.TriggerContext, context);

		public bool IsValid(int item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(Space item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(Player item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(IGameCard item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid((Space s, Player p) item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(IEnumerable<IGameCard> item, IResolutionContext context) => IsValid(context.TriggerContext, context);

		//Fulfill IListRestriction contract
		public bool AllowsValidChoice(IEnumerable<IGameCard> options, IResolutionContext context) => true;
		public IEnumerable<IGameCard> Deduplicate(IEnumerable<IGameCard> options) => options;
		public int GetMinimum(IResolutionContext context) => 0;
		public int GetMaximum(IResolutionContext context) => int.MaxValue;
		public bool IsValidClientSide(IEnumerable<IGameCard> options, IResolutionContext context) => IsValid(options, context);
		public void PrepareForSending(IResolutionContext context) { }

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			if (initializationContext.subeffect != null) useDummyResolutionContext ??= false; //Default to false if it's part of a subeffect.
			base.Initialize(initializationContext);
		}
	}
}