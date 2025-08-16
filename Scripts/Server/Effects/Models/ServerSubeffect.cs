using Kompas.Cards.Models;
using Kompas.Effects;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Models.Subeffects;
using Kompas.Server.Gamestate;
using Kompas.Shared.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models;

public abstract class ServerSubeffect : Subeffect, IServerSubeffect
{
	//TODO: protected property access to these?
	/// <summary> See <see cref="SubeffectData.forbidNotBoard"/> </summary>
	protected readonly bool forbidNotBoard;

	/// <summary> See <see cref="SubeffectData.targetIndex"/> </summary>
	private readonly int targetIndex;

	/// <summary> See <see cref="SubeffectData.spaceIndex"/> </summary>
	private readonly int spaceIndex;

	/// <summary> See <see cref="SubeffectData.cardInfoIndex"/> </summary>
	private readonly int cardInfoIndex;

	/// <summary> See <see cref="SubeffectData.playerIndex"/> </summary>
	private readonly int playerIndex;

	/// <summary> See <see cref="SubeffectData.stackableIndex"/> </summary>
	private readonly int stackableIndex;

	/// <summary> See <see cref="SubeffectData.jumpIndices"/> </summary>
	protected readonly int[]? jumpIndices;

	/// <summary> See <see cref="SubeffectData.jumpIndicesIndex"/> </summary>
	private readonly int jumpIndicesIndex;

	/// <summary> See <see cref="SubeffectData.xMultiplier"/> </summary>
	private readonly int xMultiplier;

	/// <summary> See <see cref="SubeffectData.xDivisor"/> </summary>
	private readonly int xDivisor;

	/// <summary> See <see cref="SubeffectData.xModifier"/> </summary>
	private readonly int xModifier;

	protected ServerSubeffect(SubeffectData data)
	{
		targetIndex = data.targetIndex;
		spaceIndex = data.spaceIndex;
		cardInfoIndex = data.cardInfoIndex;
		playerIndex = data.playerIndex;
		stackableIndex = data.stackableIndex;
		jumpIndices = data.jumpIndices;
		jumpIndicesIndex = data.jumpIndicesIndex;
		xMultiplier = data.xMultiplier;
		xDivisor = data.xDivisor;
		xModifier = data.xModifier;
	}

	protected override Effect _Effect => ServerEffect;
	protected override IGame _Game => ServerGame;

	private ServerEffect? _serverEffect;

	public ServerEffect ServerEffect => _serverEffect
		?? throw new NotInitializedException();
	public IServerGame ServerGame => ServerEffect.ServerGame;

	public InitializationContext DefaultInitializationContext
		=> Effect.CreateInitializationContext(this, default);

	public TargetingContext CurrTargetingContext => new()
	{
		cardTargetIndex = targetIndex,
		spaceTargetIndex = spaceIndex,
		cardInfoTargetIndex = cardInfoIndex,
		playerTargetIndex = playerIndex,
		stackableTargetIndex = stackableIndex
	};

	public virtual void Initialize(ServerEffect eff, int subeffIndex)
	{
		_serverEffect = eff;
		SubeffIndex = subeffIndex;
	}

	public abstract Task<ResolutionInfo> Resolve(ServerEffectResolution resolution);

	public virtual bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null) => true;

	public virtual Task<ResolutionInfo> OnImpossible(ServerEffectResolution resolution, string why)
	{
		var currentResolution = resolution.Context
			?? throw new EffectNotResolvingException(ServerEffect);
		currentResolution.OnImpossible = null;
		return Task.FromResult(ResolutionInfo.Impossible(why));
	}

	public virtual void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		=> ContextInitializeableBase.AdjustSubeffectIndices(jumpIndices, increment, startingAtIndex);


	/// <summary>
	/// If the effect uses X, this is the adjusted value of X
	/// </summary>
	public int AdjustX(IResolutionContext context) => (context.X * xMultiplier / xDivisor) + xModifier;

    public GameCard GetCardTarget(IResolutionContext context)
    {
        return context.GetCardTarget(targetIndex)
        	?? throw new NullCardException(TargetWasNull);
    }

    public Space GetSpaceTarget(IResolutionContext context)
    {
        return context.GetSpaceTarget(spaceIndex)
        	?? throw new NullSpaceException(TargetWasNull);
    }

    public IGameCardInfo GetCardInfoTarget(IResolutionContext context)
    {
		return EffectHelper.GetItem(context.CardInfoTargets, cardInfoIndex)
			?? throw new NullCardException(TargetWasNull);
    }

    public IPlayer GetPlayerTarget(IResolutionContext context)
    {
        return context.GetPlayerTarget(playerIndex)
        	?? throw new NullPlayerException(TargetWasNull);
    }

    public IStackable GetStackableTarget(IResolutionContext context)
    {
		return EffectHelper.GetItem(context.StackableTargets, stackableIndex)
			?? throw new NullPlayerException(TargetWasNull);
    }

    public int JumpIndex => EffectHelper.GetItem(jumpIndices
		?? throw new System.InvalidOperationException("No jump indices, but a subeffect needed one!"),
		jumpIndicesIndex);
}