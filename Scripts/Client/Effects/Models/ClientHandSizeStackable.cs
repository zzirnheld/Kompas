using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Effects.Models;

    public class ClientHandSizeStackable : HandSizeStackable, IClientStackable
    {
        public ClientHandSizeStackable(IGame game, IPlayer player) : base(game, player)
        {
        }

        public string StackableBlurb => $"{Player} must reshuffle to hand size";
    }