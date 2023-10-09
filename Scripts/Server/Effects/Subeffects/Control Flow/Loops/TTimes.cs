namespace Kompas.Server.Effects.Subeffects
{
	public class TTimesSubeffect : Loop
	{
		public int T;
		private int count = 0;

		protected override void OnLoopExit()
		{
			base.OnLoopExit();
			count = 0;
		}

		protected override bool ShouldContinueLoop
		{
			get
			{
				count++;
				return count < T;
			}
		}
	}
}