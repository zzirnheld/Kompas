using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Server.Gamestate;

namespace Kompas.Server.Effects.Models.Subeffects;

public interface IServerSubeffect
{

	public ServerEffect ServerEffect { get; }
	public IServerGame ServerGame { get; }
	public int JumpIndex { get; }
	public int SubeffIndex { get; }

	public InitializationContext DefaultInitializationContext { get; }

	/// <summary>
	/// Sets up the subeffect with whatever necessary values.
	/// Usually also initializes any restrictions the effects are using.
	/// </summary>
	/// <param name="eff">The effect this subeffect is part of.</param>
	/// <param name="subeffIndex">The index in the subeffect array of its parent <paramref name="eff"/> this subeffect is.</param>
	public void Initialize(ServerEffect eff, int subeffIndex);

	/// <summary>
	/// Server Subeffect resolve method. Does whatever this type of subeffect does
	/// <returns>A ResolutionInfo object describing what to do next</returns>
	/// </summary>
	public Task<ResolutionInfo> Resolve(ServerEffectResolution resolution);

	/// <summary>
	/// Whether this subeffect will be considered EffectImpossible at this point
	/// </summary>
	/// <returns></returns>
	public bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null);

	/// <summary>
	/// Optional method. If implemented, does something when the effect is declared impossible.
	/// Default implementation just finishes resolution of the effect
	/// </summary>
	public Task<ResolutionInfo> OnImpossible(ServerEffectResolution resolution, string why);

	public void AdjustSubeffectIndices(int increment, int startingAtIndex = 0);


	/// <summary>
	/// If the effect uses X, this is the adjusted value of X
	/// </summary>
	public int AdjustX(IResolutionContext context);

	public GameCard GetCardTarget(IResolutionContext context);

	public Space GetSpaceTarget(IResolutionContext context);

	public IGameCardInfo GetCardInfoTarget(IResolutionContext context);

	public IPlayer GetPlayerTarget(IResolutionContext context);

	public IStackable GetStackableTarget(IResolutionContext context);
	
}