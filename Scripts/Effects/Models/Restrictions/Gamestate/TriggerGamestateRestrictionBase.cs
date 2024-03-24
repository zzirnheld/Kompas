using System.Collections.Generic;
using Godot;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	/// <summary>
	/// Base class for trigger restrictions that can also act like gamestate restrictions
	/// </summary>
	public abstract class TriggerGamestateRestrictionBase : ContextInitializeableBase, IGamestateRestriction, ITriggerRestriction
	{
		public bool IsValid(int item, IResolutionContext context) => IsValid(context);
		public bool IsValid(Space? item, IResolutionContext context) => IsValid(context);
		public bool IsValid(IPlayer? item, IResolutionContext context) => IsValid(context);
		public bool IsValid(IGameCardInfo? item, IResolutionContext context) => IsValid(context);
		public bool IsValid((Space? s, IPlayer? p) item, IResolutionContext context) => IsValid(context);
		public bool IsValid(IEnumerable<IGameCardInfo>? item, IResolutionContext context) => IsValid(context);

		public bool IsValid(IResolutionContext context) => IsValid(context, context);

		//This one is special - we want to use a dummy not resolving context for the first one because we might want to consult the second one if using stashed context,
		//Like for a hanging effect that needs to resolve.
		//Since we do it this way,
		//the primary context passed into IsValidLogic will always be a valid current one (or, well, if it's invalid it'll be because we're testing a player action),
		//and the second one will either duplicate it, or be the secondary context in the case where it's applicable (which is where we're acting like a trigger restriction)
		public bool IsValid(TriggeringEventContext? triggeringEventContext, IResolutionContext resolutionContext)
			=> IsValid(IResolutionContext.NotResolving(triggeringEventContext), resolutionContext);

		private bool IsValid(IResolutionContext context, IResolutionContext secondaryContext)
		{
			ComplainIfNotInitialized();

			try { return IsValidLogic(context, secondaryContext); }
			catch (System.SystemException exception)
				when (exception is System.NullReferenceException || exception is System.ArgumentException)
			{
				GD.PrintErr(exception);
				return false;
			}
		}

        /// <param name="context">The primary resolution context to be considering.
        /// Reflects the current state of what's going on in terms of effects/player actions.</param>
        /// <param name="secondaryContext">A secondary context we might want to consider,
        /// usually one that's been stashed from another time stuff was happening, ex. for hanging effects. </param>
        /// <returns></returns>
		protected abstract bool IsValidLogic(IResolutionContext context, IResolutionContext secondaryContext);

		//Fulfill trigger restriction contract.
		//Because this fulfills the trigger resolution contract, the IResolutionContext will always be a dummy,
		//because it's being called while determining what should go on the stack, between resolutions of effects.
		public abstract bool IsStillValidTriggeringContext(TriggeringEventContext context);

		//Fulfill list restriction contract
		public bool AllowsValidChoice(IEnumerable<IGameCardInfo> options, IResolutionContext context) => true;
		public IEnumerable<IGameCardInfo> Deduplicate(IEnumerable<IGameCardInfo> options) => options;
		public int GetMinimum(IResolutionContext? context) => 0;
		public int GetMaximum(IResolutionContext? context) => int.MaxValue;
		public bool IsValidClientSide (IEnumerable<IGameCardInfo>? options, IResolutionContext context) => IsValid(options, context);
		public void PrepareForSending(IResolutionContext context) { }
	}
}