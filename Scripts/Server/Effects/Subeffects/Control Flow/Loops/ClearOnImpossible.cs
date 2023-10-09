using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	/// <summary>
	/// Removes any effect currently set to trigger if an effect is declared impossible.
	/// </summary>
	public class ClearOnImpossible : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			ServerEffect.OnImpossible = null;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}