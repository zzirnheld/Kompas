using System;
using System.Collections.Generic;

namespace Kompas.Effects.Models.Restrictions.Triggering
{
	public abstract class TriggerRestrictionBase : RestrictionBase<TriggeringEventContext>
	{
		public static readonly IRestriction<TriggeringEventContext>[] DefaultFallOffRestrictions = {
			new TriggerRestrictions.CardsMatch(){
				card = new Identities.Cards.ThisCardNow(),
				other = new Identities.Cards.CardBefore()
			},
			new GamestateRestrictions.ThisCardInPlay() };

		public static readonly ISet<Type> ReevalationRestrictions = new HashSet<Type>(new Type[] {
			typeof(GamestateRestrictionElements.MaxPerTurn),
			typeof(GamestateRestrictionElements.MaxPerRound),
			typeof(GamestateRestrictionElements.MaxPerStack)
		});

		public static IRestriction<TriggeringEventContext> AllOf(IList<IRestriction<TriggeringEventContext>> elements)
			=> new TriggerRestrictionElements.AllOf() { elements = elements };

		public bool? useDummyResolutionContext;

		public bool UseDummyResolutionContext
		{
			get
			{
				if (!useDummyResolutionContext.HasValue) throw new ArgumentNullException(nameof(useDummyResolutionContext),
					"You tried to check whether we should use a dummy ResolutionContext before initializing the restriction!");

				return useDummyResolutionContext.Value;
			}
		}

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			useDummyResolutionContext ??= true; //Default to true, but in a way that can be overridden by child classes like TriggerGamestateRestrictionBase
		}

		protected virtual IResolutionContext ContextToConsider(TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
			=> UseDummyResolutionContext
				? IResolutionContext.Dummy(triggeringContext)
				: resolutionContext;
	}
}