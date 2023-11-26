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
		public bool IsValid(Space? item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(IPlayer? item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(IGameCardInfo? item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid((Space? s, IPlayer? p) item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(IEnumerable<IGameCardInfo>? item, IResolutionContext context) => IsValid(context.TriggerContext, context);

		//Fulfill IListRestriction contract
		public bool AllowsValidChoice(IEnumerable<IGameCardInfo> options, IResolutionContext context) => true;
		public IEnumerable<IGameCardInfo> Deduplicate(IEnumerable<IGameCardInfo> options) => options;
		public int GetMinimum(IResolutionContext? context) => 0;
		public int GetMaximum(IResolutionContext? context) => int.MaxValue;
		public bool IsValidClientSide (IEnumerable<IGameCardInfo>? options, IResolutionContext context) => IsValid(options, context);
		public void PrepareForSending(IResolutionContext context) { }

        public override void Initialize(EffectInitializationContext initializationContext)
		{
			if (initializationContext.subeffect != null) useDummyResolutionContext ??= false; //Default to false if it's part of a subeffect.
			base.Initialize(initializationContext);
		}
	}
}