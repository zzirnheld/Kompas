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

		/// <summary>
		/// Gets the cause of some downstream outcome of the attack,
		/// w/r/t whatever card that downstream outcome happened to.
		/// DO NOT USE for getting the cause of the attack starting/ending,
		/// the cause of that is always the card that initiated the attack.
		/// </summary>
		public GameCard? GetCause(IGameCardInfo? withRespectTo)
		{
			if (withRespectTo == null)
			{
				Logger.Warn($"Tried to get cause w/r/t null card for attack {this}");
				return null;
			}
			else if (attacker == withRespectTo.Card) return defender;
			else if (defender == withRespectTo.Card) return attacker;
			else if (attacker == withRespectTo.AugmentedCard) return defender;
			else if (defender == withRespectTo.AugmentedCard) return attacker;
			else throw new System.ArgumentException($"Why is {withRespectTo} neither the attacker {attacker} nor defender {defender}, nor augmenting them, " +
				$"in the attack {this} that caused something to happen to it?");
		}
	}
}