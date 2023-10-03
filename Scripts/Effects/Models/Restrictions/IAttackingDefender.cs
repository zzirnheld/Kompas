using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions.Cards;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IAttackingDefender : IRestriction<IGameCard>
	{
		public static IAttackingDefender CreateDefault() => new AttackingDefender();
	}
}