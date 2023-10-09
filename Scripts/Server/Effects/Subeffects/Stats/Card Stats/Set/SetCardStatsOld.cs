using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Subeffects
{
	public class SetCardStatsOld : SetCardStats
	{
		public int nVal = -1;
		public int eVal = -1;
		public int sVal = -1;
		public int wVal = -1;
		public int cVal = -1;
		public int aVal = -1;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			if (nVal >= 0) n ??= new Constant() { constant = nVal };
			if (eVal >= 0) n ??= new Constant() { constant = eVal };
			if (sVal >= 0) n ??= new Constant() { constant = sVal };
			if (wVal >= 0) n ??= new Constant() { constant = wVal };
			if (cVal >= 0) n ??= new Constant() { constant = cVal };
			if (aVal >= 0) n ??= new Constant() { constant = aVal };

			base.Initialize(eff, subeffIndex);
		}
	}
}