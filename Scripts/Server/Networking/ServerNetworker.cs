using Godot;
using Kompas.Networking;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Kompas.Server.Networking
{
	//handles networking and such for a server game
	public class ServerNetworker : NetworkController
	{
		public static readonly string[] DontLogThesePackets =
		{
			Packet.PassPriority
		};

		public ServerPlayer Player;
		public ServerGame sGame;
		public ServerNotifier ServerNotifier;
		public ServerAwaiter serverAwaiter;

		public ServerNetworker(TcpClient tcpClient) : base(tcpClient)
		{
		}

		private IServerOrderPacket FromJson(string command, string json)
		{
			return command switch
			{
				//game start
				Packet.SetDeck => JsonConvert.DeserializeObject<SetDeckServerPacket>(json),
				//player actions
				Packet.PlayAction => JsonConvert.DeserializeObject<PlayActionServerPacket>(json),
				Packet.AugmentAction => JsonConvert.DeserializeObject<AugmentActionServerPacket>(json),
				Packet.MoveAction => JsonConvert.DeserializeObject<MoveActionServerPacket>(json),
				Packet.AttackAction => JsonConvert.DeserializeObject<AttackActionServerPacket>(json),
				Packet.EndTurnAction => JsonConvert.DeserializeObject<EndTurnActionServerPacket>(json),
				Packet.ActivateEffectAction => JsonConvert.DeserializeObject<ActivateEffectActionServerPacket>(json),
				Packet.HandSizeChoices => JsonConvert.DeserializeObject<SendHandSizeChoicesServerPacket>(json),
				//effects
				Packet.CardTargetChosen => JsonConvert.DeserializeObject<CardTargetServerPacket>(json),
				Packet.SpaceTargetChosen => JsonConvert.DeserializeObject<SpaceTargetServerPacket>(json),
				Packet.XSelectionChosen => JsonConvert.DeserializeObject<SelectXServerPacket>(json),
				Packet.DeclineAnotherTarget => JsonConvert.DeserializeObject<DeclineAnotherTargetServerPacket>(json),
				Packet.ListChoicesChosen => JsonConvert.DeserializeObject<ListChoicesServerPacket>(json),
				Packet.OptionalTriggerResponse => JsonConvert.DeserializeObject<OptionalTriggerAnswerServerPacket>(json),
				Packet.ChooseEffectOption => JsonConvert.DeserializeObject<EffectOptionResponseServerPacket>(json),
				Packet.PassPriority => JsonConvert.DeserializeObject<PassPriorityServerPacket>(json),
				Packet.ChooseTriggerOrder => JsonConvert.DeserializeObject<TriggerOrderResponseServerPacket>(json),
				//debug
				Packet.DebugTopdeck => JsonConvert.DeserializeObject<DebugTopdeckServerPacket>(json),
				Packet.DebugDiscard => JsonConvert.DeserializeObject<DebugDiscardServerPacket>(json),
				Packet.DebugRehand => JsonConvert.DeserializeObject<DebugRehandServerPacket>(json),
				Packet.DebugDraw => JsonConvert.DeserializeObject<DebugDrawServerPacket>(json),
				Packet.DebugSetNESW => JsonConvert.DeserializeObject<DebugSetNESWServerPacket>(json),
				Packet.DebugSetPips => JsonConvert.DeserializeObject<DebugSetPipsServerPacket>(json),
				//misc
				_ => throw new System.ArgumentException($"Unrecognized command {command} in packet sent to client"),
			};
		}

		public override async void Tick()
		{
			//GD.Print("SERVER NET CTRL UPDATE");
			base.Tick();
			if (packets.Count != 0) await ProcessPacket(packets.Dequeue());
			//if (sGame.Players.Any(p => p.TcpClient != null && !p.TcpClient.Connected)) Destroy(sGame.gameObject); //TODO notify player that no
		}

		public override async Task ProcessPacket((string command, string json) packetInfo)
		{
			if (packetInfo.command == Packet.Invalid)
			{
				GD.PrintErr($"Invalid packet {packetInfo.json}");
				return;
			}

			if (!DontLogThesePackets.Contains(packetInfo.command)) GD.Print($"Processing {packetInfo.json} from {Player}");

			var packet = FromJson(packetInfo.command, packetInfo.json);
			await packet.Execute(sGame, Player, serverAwaiter);
		}
	}
}