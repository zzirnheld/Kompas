using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class XTimes : Loop
	{
		private int count = 0;

		protected override bool ShouldContinueLoop
		{
			get
			{
				count++;
				return count < ServerEffect.X;
			}
		}

		protected override void OnLoopExit()
		{
			base.OnLoopExit();
			count = 0;
		}

		public override Task<ResolutionInfo> OnImpossible(string why)
		{
			count = 0;
			return base.OnImpossible(why);
		}
	}
}