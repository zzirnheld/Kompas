﻿using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class AttackStartedPacket : Packet
    {
        public int attackerId;
        public int defenderId;
        public int controllerIndex;

        public AttackStartedPacket() : base(AttackStarted) { }

        public AttackStartedPacket(int attackerId, int defenderId, int controllerIndex) : this()
        {
            this.attackerId = attackerId;
            this.defenderId = defenderId;
            this.controllerIndex = controllerIndex;
        }

        public override Packet Copy() => new AttackStartedPacket(attackerId, defenderId, controllerIndex);

        public override Packet GetInversion(bool known = true) => new AttackStartedPacket(attackerId, defenderId, 1 - controllerIndex);
    }
}

namespace KompasClient.Networking
{
    public class AttackStartedClientPacket : AttackStartedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var attacker = clientGame.GetCardWithID(attackerId);
            var defender = clientGame.GetCardWithID(defenderId);
            var controller = clientGame.Players[controllerIndex];
            if (attacker != null && defender != null)
            {
                clientGame.clientUICtrl.SetCurrState("Attack Started", $"{attacker.CardName} attacks {defender.CardName}");
                clientGame.clientEffectsCtrl.Add(new ClientAttack(controller, attacker: attacker, defender: defender));
            }

            //if (card != null) card.AttacksThisTurn++;
            //don't do this because AttacksThisTurn should be updated by server and told to client
        }
    }
}