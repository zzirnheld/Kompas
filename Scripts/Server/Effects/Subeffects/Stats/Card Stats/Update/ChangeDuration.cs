using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ChangeDuration : UpdateCardStats
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			duration ??= new EffectX();
			base.Initialize(eff, subeffIndex);
		}
	}
}