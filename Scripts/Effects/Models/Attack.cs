using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public abstract class Attack : IStackable
	{
		public readonly IPlayer instigator;
		public readonly GameCard attacker;
		public readonly GameCard defender;

		public GameCard Card => attacker;

		public IPlayer? ControllingPlayer => instigator;

		/// <summary>
		/// Constructor should be called when the attack is declared
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="defender"></param>
		public Attack(IPlayer instigator, GameCard attacker, GameCard defender)
		{
			this.instigator = instigator ?? throw new System.ArgumentNullException(nameof(instigator), "Cannot have null controller of attack");
			this.attacker = attacker ?? throw new System.ArgumentNullException(nameof(attacker), "Cannot have null attacker");
			this.defender = defender ?? throw new System.ArgumentNullException(nameof(defender), "Cannot have null defender");
		}

		public GameCard? GetCause(IGameCardInfo? withRespectTo)
		{
			if (withRespectTo == null) throw new System.ArgumentNullException(nameof(withRespectTo), "Why did you try and get the cause of an attack w/r/t a null card?");
			else if (attacker == withRespectTo.Card) return defender;
			else if (defender == withRespectTo.Card) return attacker;
			else if (attacker == withRespectTo.AugmentedCard) return defender;
			else if (defender == withRespectTo.AugmentedCard) return attacker;
			else throw new System.ArgumentException($"Why is {withRespectTo} neither the attacker {attacker} nor defender {defender}, nor augmenting them, " +
				$"in the attack {this} that caused something to happen to it?");
		}
	}
}