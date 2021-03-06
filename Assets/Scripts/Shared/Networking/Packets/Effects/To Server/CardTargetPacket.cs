﻿using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class CardTargetPacket : Packet
    {
        public int cardId;

        public CardTargetPacket() : base(CardTargetChosen) { }

        public CardTargetPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new CardTargetPacket(cardId);
    }
}

namespace KompasServer.Networking
{
    public class CardTargetServerPacket : CardTargetPacket, IServerOrderPacket
    {

        public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            awaiter.CardTarget = serverGame.GetCardWithID(cardId);
            return Task.CompletedTask;
        }
    }
}