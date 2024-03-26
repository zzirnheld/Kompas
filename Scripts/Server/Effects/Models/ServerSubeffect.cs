using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Shared.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models
{
	public abstract class ServerSubeffect : Subeffect
	{
		protected override Effect _Effect => ServerEffect;
		protected override IGame _Game => ServerGame;

		private ServerEffect? _serverEffect;
		public ServerEffect ServerEffect => _serverEffect
			?? throw new NotInitializedException();
		public IServerGame ServerGame => ServerEffect.ServerGame;

		public InitializationContext DefaultInitializationContext
			=> Effect.CreateInitializationContext(this, default);

		/// <summary>
		/// Sets up the subeffect with whatever necessary values.
		/// Usually also initializes any restrictions the effects are using.
		/// </summary>
		/// <param name="eff">The effect this subeffect is part of.</param>
		/// <param name="subeffIndex">The index in the subeffect array of its parent <paramref name="eff"/> this subeffect is.</param>
		public virtual void Initialize(ServerEffect eff, int subeffIndex)
		{
			//Logger.Log($"Finishing setup for new subeffect of type {GetType()}");
			_serverEffect = eff;
			SubeffIndex = subeffIndex;
			if (xMultiplier == 1 && xModifier != 0) Logger.Log($"x mulitplier {xMultiplier}, relies on default on eff of {Effect.Card}");
		}

		/// <summary>
		/// Server Subeffect resolve method. Does whatever this type of subeffect does
		/// <returns>A ResolutionInfo object describing what to do next</returns>
		/// </summary>
		public abstract Task<ResolutionInfo> Resolve();

		/// <summary>
		/// Whether this subeffect will be considered EffectImpossible at this point
		/// </summary>
		/// <returns></returns>
		public virtual bool IsImpossible(TargetingContext? overrideContext = null) => true;


		/// <summary>
		/// Optional method. If implemented, does something when the effect is declared impossible.
		/// Default implementation just finishes resolution of the effect
		/// </summary>
		public virtual Task<ResolutionInfo> OnImpossible(string why)
		{
			ServerEffect.OnImpossible = null;
			return Task.FromResult(ResolutionInfo.Impossible(why));
		}

		public virtual void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
			=> ContextInitializeableBase.AdjustSubeffectIndices(jumpIndices, increment, startingAtIndex);
	}

	public struct ResolutionInfo
	{
		public const string EndedBecauseImpossible = "Ended because effect was impossible";

		public ResolutionResult result;

		public int index;

		public string reason;

		public static ResolutionInfo Next => new ResolutionInfo { result = ResolutionResult.Next };
		public static ResolutionInfo Index(int index) => new ResolutionInfo { result = ResolutionResult.Index, index = index };
		public static ResolutionInfo Impossible(string why) => new ResolutionInfo { result = ResolutionResult.Impossible, reason = why };
		public static ResolutionInfo End(string why) => new ResolutionInfo { result = ResolutionResult.End, reason = why };
	}

	public enum ResolutionResult
	{
		Next, Index, Impossible, End
	}
}