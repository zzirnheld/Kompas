using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Effects.Models
{
	public class ClientAttack : Attack, IClientStackable
	{
		public ClientAttack(IPlayer instigator, GameCard attacker, GameCard defender)
			: base(instigator, attacker, defender)
		{
		}

		public string StackableBlurb => $"{attacker.CardName} attacks {defender.CardName}";
	}
}