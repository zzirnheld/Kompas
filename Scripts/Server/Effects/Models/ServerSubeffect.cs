using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Server.Gamestate;
using Kompas.Effects.Subeffects;
using Kompas.Shared.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models;

public abstract class ServerSubeffect<DataType> : Subeffect<DataType>
	where DataType : SubeffectData
{
	protected override Effect _Effect => ServerEffect;
	protected override IGame _Game => ServerGame;

	private ServerEffect? _serverEffect;

	public ServerEffect ServerEffect => _serverEffect
		?? throw new NotInitializedException();
	public IServerGame ServerGame => ServerEffect.ServerGame;

	public InitializationContext DefaultInitializationContext
		=> Effect.CreateInitializationContext(this, default);

	protected ServerSubeffect(DataType data) : base(data)
	{
	}

	/// <summary>
	/// Sets up the subeffect with whatever necessary values.
	/// Usually also initializes any restrictions the effects are using.
	/// </summary>
	/// <param name="eff">The effect this subeffect is part of.</param>
	/// <param name="subeffIndex">The index in the subeffect array of its parent <paramref name="eff"/> this subeffect is.</param>
	public virtual void Initialize(ServerEffect eff, int subeffIndex)
	{
		_serverEffect = eff;
		SubeffIndex = subeffIndex;
	}

	/// <summary>
	/// Server Subeffect resolve method. Does whatever this type of subeffect does
	/// <returns>A ResolutionInfo object describing what to do next</returns>
	/// </summary>
	public abstract Task<ResolutionInfo> Resolve(ServerEffectResolution resolution);

	/// <summary>
	/// Whether this subeffect will be considered EffectImpossible at this point
	/// </summary>
	/// <returns></returns>
	public virtual bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null) => true;

	/// <summary>
	/// Optional method. If implemented, does something when the effect is declared impossible.
	/// Default implementation just finishes resolution of the effect
	/// </summary>
	public virtual Task<ResolutionInfo> OnImpossible(ServerEffectResolution resolution, string why)
	{
		var currentResolution = resolution.Context
			?? throw new EffectNotResolvingException(ServerEffect);
		currentResolution.OnImpossible = null;
		return Task.FromResult(ResolutionInfo.Impossible(why));
	}

	public virtual void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		=> ContextInitializeableBase.AdjustSubeffectIndices(Data.jumpIndices, increment, startingAtIndex);
}