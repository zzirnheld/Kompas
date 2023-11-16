using System;
using System.Collections.Generic;
using Godot;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public abstract class GamestateRestrictionBase : ContextInitializeableBase, IGamestateRestriction
	{
		public bool IsValid(int item, IResolutionContext context) => IsValid(context);
		public bool IsValid(Space item, IResolutionContext context) => IsValid(context);
		public bool IsValid(IPlayer item, IResolutionContext context) => IsValid(context);
		public bool IsValid(IGameCardInfo item, IResolutionContext context) => IsValid(context);
		public bool IsValid((Space? s, IPlayer? p) item, IResolutionContext context) => IsValid(context);
		public bool IsValid(TriggeringEventContext item, IResolutionContext context) => IsValid(context);
		public bool IsValid(IEnumerable<IGameCardInfo> item, IResolutionContext context) => IsValid(context);

		public bool IsValid(IResolutionContext context)
		{
			ComplainIfNotInitialized();

			try { return IsValidLogic(context); }
			catch (SystemException exception)
				when (exception is NullReferenceException || exception is ArgumentException)
			{
				GD.PrintErr(exception);
				return false;
			}
		}

		protected abstract bool IsValidLogic(IResolutionContext context);

		//Fulfill list restriction contract
		public bool AllowsValidChoice(IEnumerable<IGameCardInfo> options, IResolutionContext context) => true;
		public IEnumerable<IGameCardInfo> Deduplicate(IEnumerable<IGameCardInfo> options) => options;
		public int GetMinimum(IResolutionContext context) => 0;
		public int GetMaximum(IResolutionContext context) => int.MaxValue;
		public bool IsValidClientSide(IEnumerable<IGameCardInfo> options, IResolutionContext context) => IsValid(options, context);
		public void PrepareForSending(IResolutionContext context) { }
	}
}