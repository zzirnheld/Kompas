using System;
using System.Collections.Generic;

namespace Kompas.Effects.Models.Restrictions.Triggering
{
	public abstract class TriggerRestrictionBase : RestrictionBase<TriggeringEventContext>, ITriggerRestriction
	{
		public static readonly ITriggerRestriction[] DefaultFallOffRestrictions = {
			new Gamestate.CardsMatch(){
				card = new Identities.Cards.ThisCardNow(),
				other = new Identities.Cards.CardBefore()
			},
			new Gamestate.ThisCardInPlay() };

		public static readonly ISet<Type> ReevalationRestrictions = new HashSet<Type>(new Type[] {
			typeof(Gamestate.MaxPerTurn),
			typeof(Gamestate.MaxPerRound),
			typeof(Gamestate.MaxPerStack)
		});

		public static ITriggerRestriction AllOf(IList<ITriggerRestriction> elements)
			//Compiler needed the help to know that an ITriggerRestriction is an IRestriction<TriggeringEventContext>
			=> new AllOf() { elements = elements };

		protected override sealed bool IsValidLogic(TriggeringEventContext? item, IResolutionContext context)
		{
	   		var NullTriggeringContext = "Triggering event context was null? If you see this, consider if it's allowable";
			_ = item ?? throw new System.ArgumentNullException(NullTriggeringContext);
			return IsValidContext(item, context);
		}
		
		protected abstract bool IsValidContext(TriggeringEventContext item, IResolutionContext context);

		/// <summary>
        /// If IsValidContext initially evaluated to true, is this restriction still valid after other triggers have made it onto the stack?
        /// <br/>
        /// IMPL Notes:<br/>
        /// Return true if the state won't change based JUST on items going onto the stack.
        /// Evaluate the restriction again if items going onto the stack could affect whether this is valid.
        /// </summary>
		public abstract bool IsStillValidTriggeringContext(TriggeringEventContext context);
	}
}