using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Server.Effects.Models;

public class ServerAttack : Attack, IServerStackable
{
    public ServerAttack(IPlayer instigator, GameCard attacker, GameCard defender)
		: base(instigator, attacker, defender)
	{ }
}