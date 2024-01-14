using System.Threading.Tasks;
using Godot;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SetX : ServerSubeffect
	{
		public virtual int BaseCount => Effect.X;

		public int TrueCount => (BaseCount * xMultiplier / xDivisor) + xModifier + (change ? Effect.X : 0);

		public bool change = false;

		public override Task<ResolutionInfo> Resolve()
		{
			var context = Effect.CurrentResolutionContext ?? throw new EffectNotResolvingException(Effect);
			context.X = TrueCount;
			GD.Print($"Setting X to {Effect.X}");
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}