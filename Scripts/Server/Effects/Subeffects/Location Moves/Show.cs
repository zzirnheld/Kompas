using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	/// <summary>
	/// Like a RevealSubeffect, but isn't impossible if the card is already revealed.
	/// </summary>
	public class Show : ServerSubeffect
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> GetCardTarget(overrideContext) == null;

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null) throw new NullCardException(TargetWasNull);

			try { CardTarget.Reveal(Effect); }
			catch (AlreadyKnownException) { }

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}