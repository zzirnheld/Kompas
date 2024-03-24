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
		public bool IsValid(TriggeringEventContext? item, IResolutionContext context) => IsValid(context);

		public bool IsValid(IResolutionContext context)
		{
			ComplainIfNotInitialized();

			try { return IsValidLogic(context); }
			catch (System.SystemException exception)
				when (exception is System.NullReferenceException || exception is System.ArgumentException)
			{
				GD.PrintErr(exception);
				return false;
			}
		}

		protected abstract bool IsValidLogic(IResolutionContext context);

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