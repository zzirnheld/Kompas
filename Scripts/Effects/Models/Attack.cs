using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public abstract class Attack : IStackable
	{
		public readonly Player controller;
		public readonly GameCard attacker;
		public readonly GameCard defender;

		public GameCard Source => attacker;
		public Player Controller => controller;

		/// <summary>
		/// Constructor should be called when the attack is declared
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="defender"></param>
		public Attack(Player controller, GameCard attacker, GameCard defender)
		{
			this.controller = controller != null ? controller : throw new System.ArgumentNullException("controller", "Cannot have null controller of attack");
			this.attacker = attacker != null ? attacker : throw new System.ArgumentNullException("attacker", "Cannot have null attacker");
			this.defender = defender != null ? defender : throw new System.ArgumentNullException("defender", "Cannot have null defender");
		}

		public GameCard GetCause(GameCardBase withRespectTo)
		{
			if (withRespectTo == null) throw new System.ArgumentNullException("Why did you try and get the cause of an attack w/r/t a null card?");
			else if (attacker == withRespectTo.Card) return defender;
			else if (defender == withRespectTo.Card) return attacker;
			else if (attacker == withRespectTo.AugmentedCard) return defender;
			else if (defender == withRespectTo.AugmentedCard) return attacker;
			else throw new System.ArgumentException($"Why is {withRespectTo} neither the attacker {attacker} nor defender {defender}, nor augmenting them, " +
				$"in the attack {this} that caused something to happen to it?");
		}
	}
}