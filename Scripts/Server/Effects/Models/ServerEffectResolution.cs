using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;
using Kompas.Server.Networking;

namespace Kompas.Server.Effects.Models;

public class ServerEffectResolution
	: StackableResolution<IServerEffect, IServerResolutionContext>,
		IServerStackableResolution<IServerEffect>
{
	public const string EffectWasNegated = "Effect was negated";

	public IServerEffect Effect => Stackable;

	public int SubeffectIndex { get; set; }

	private GameCard Card => Effect.Card ?? throw new NullCardException("effect must be on a card!");

	public ServerEffectResolution(IServerEffect effect, IServerResolutionContext context)
		: base(effect, context)
	{ }

	public async Task StartResolution()
	{
		Logger.Log($"Resolving effect {Effect.EffectIndex} of {Card.CardName} in context {Context}");

		//Notify the targets one by one so the client knows that they're current targets
		if (Context.CardTargets != null) foreach (var tgt in Context.CardTargets) NotifyAddCardTarget(tgt);

		Context.playerTargets.Add(Context.ControllingPlayer);
		if (Context.TriggerContext?.StackableCause != null) Context.StackableTargets.Add(Context.TriggerContext.StackableCause);

		//notify relevant to this effect starting
		ServerNotifier.NotifyEffectX(Card, Effect.EffectIndex, Context.X, Effect.Game.Players);
		ServerNotifier.EffectResolving(Context.ControllingPlayer, Effect);

		//resolve the effect if possible
		if (Effect.Negated) await EffectImpossible(EffectWasNegated);
		else await Resolve(Context.StartIndex);

		//after all subeffects have finished, clean up
		FinishResolution();

		//then return. server effects controller will interpret returning as effect being done.
	}

	private async Task Resolve(int index)
	{
		//get first result
		ResolutionInfo result = await ResolveSubeffect(index);

		//then, so long as we should keep going, resolve subeffects
		bool resolve = true;
		while (resolve)
		{
			switch (result.result)
			{
				case ResolutionResult.Next:
					index++;
					if (index < Effect.ServerSubeffects.Length) result = await ResolveSubeffect(index);
					else resolve = false; //stop if next subeffect index is out of bounds
					break;
				case ResolutionResult.Index:
					index = result.index;
					if (index < Effect.ServerSubeffects.Length) result = await ResolveSubeffect(index);
					else resolve = false; //stop if that subeffect index is out of bounds
					break;
				case ResolutionResult.Impossible:
					Logger.Log($"Effect of {Effect.Card?.CardName} was impossible at index {index} because {result.reason}. Going to OnImpossible if applicable");
					result = await EffectImpossible(result.reason);
					break;
				case ResolutionResult.End:
					//TODO send to player why resolution ended (including "[cardname] effect finished resolving")
					Logger.Log($"Finished resolution of effect of {Effect.Card?.CardName} because {result.reason}");
					resolve = false;
					break;
				default:
					throw new System.ArgumentException($"Invalid resolution result {result.result}");
			}
		}
	}

	public async Task<ResolutionInfo> ResolveSubeffect(int index)
	{
		if (index >= Effect.ServerSubeffects.Length)
		{
			return ResolutionInfo.Impossible("Subeffect index out of bounds.");
		}

		Logger.Log($"Resolving subeffect of type {Effect.ServerSubeffects[index].GetType()}");
		SubeffectIndex = index;
		ServerNotifier.NotifyEffectX(Card, Effect.EffectIndex, Context.X, Effect.Game.Players);
		try
		{
			return await Effect.ServerSubeffects[index].Resolve(this);
		}
		catch (KompasException e)
		{
			Logger.Warn($"Caught {e.GetType()} while resolving {Effect.ServerSubeffects[index].GetType()} at {index}." +
				$"\nStack trace:\n{e.StackTrace}");
			return ResolutionInfo.Impossible(e.Message);
		}
	}

	/// <summary>
	/// If the effect finishes resolving, this method is called.
	/// </summary>
	private void FinishResolution()
	{
		SubeffectIndex = 0;

		// CurrentServerResolutionContext = null;
		//TODO: analogous operation on ServerEffect?

		ServerNotifier.NotifyBothPutBack(Effect.Game.Players);
		foreach (var p in Effect.Game.Players) ServerNotifier.DisableDecliningTarget(p);
	}

	/// <summary>
	/// Cancels resolution of the effect, 
	/// or, if there is something pending if the effect becomes impossible, resolves that
	/// </summary>
	public async Task<ResolutionInfo> EffectImpossible(string why)
	{
		Logger.Log($"Effect of {Card.CardName} is being declared impossible at subeffect {Effect.ServerSubeffects[SubeffectIndex].GetType()} because {why}");
		if (Context.OnImpossible == null)
		{
			//TODO make the notifier tell the client why the effect was impossible
			ServerNotifier.EffectImpossible(Effect.Game.Players);
			foreach (var p in Effect.Game.Players) ServerNotifier.DisableDecliningTarget(p);

			return ResolutionInfo.End(ResolutionInfo.EndedBecauseImpossible);
		}
		else
		{
			SubeffectIndex = Context.OnImpossible.SubeffIndex;
			return await Context.OnImpossible.OnImpossible(this, why);
		}
	}

	public void AddTarget(GameCard target, IPlayer? onlyOneToKnow = null)
	{
		Context.CardTargets.Add(target);
		NotifyAddCardTarget(target, onlyOneToKnow);
	}

	private void NotifyAddCardTarget(GameCard target, IPlayer? onlyOneToKnow = null)
	{
		if (onlyOneToKnow != null) ServerNotifier.AddHiddenTarget(onlyOneToKnow, Card, Effect.EffectIndex, target);
		else ServerNotifier.AddTarget(Card, Effect.EffectIndex, target, Effect.Game.Players);
	}

	public void RemoveTarget(GameCard target)
	{
		Context.CardTargets.Remove(target);
		ServerNotifier.RemoveTarget(Card, Effect.EffectIndex, target, Effect.Game.Players);
	}

	public void AddSpace(Space space) => Context.SpaceTargets.Add(space.Copy);
}