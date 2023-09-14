using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IAttackingDefender : IRestriction<GameCardBase>
	{
		public static IAttackingDefender CreateDefault() => new AttackingDefender();
	}
}