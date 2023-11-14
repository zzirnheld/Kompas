using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions.Cards;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IAttackingDefender : IRestriction<IGameCardInfo>
	{
		public static IAttackingDefender CreateDefault() => new AttackingDefender();
	}
}