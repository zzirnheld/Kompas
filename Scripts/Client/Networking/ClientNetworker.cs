using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using Kompas.Client.Gamestate;
using Kompas.Networking;
using Newtonsoft.Json;

namespace Kompas.Client.Networking
{
	public class ClientNetworker : Networker
	{
		private static readonly JsonSerializerSettings DeserializePacketJsonSettings = new() { TypeNameHandling = TypeNameHandling.Auto };

		private readonly ClientGame game;
		private static bool connecting = false;

		public ClientNetworker(TcpClient tcpClient, ClientGame game)
			: base(tcpClient)
		{
			this.game = game;
		}

		//TODO workflow:
		/*
		* GameController calls connect.
		* It catches the socket exception and re shows choose server if so
		* Based on whether the tcpClient is non-null and connected, it shows waiting for player;
		* else choose server again.
		*/
		public static async Task<TcpClient?> Connect(string ip)
		{
			GD.Print($"Connect to {ip}?");
			if (connecting) return null;

			connecting = true;
			
			GD.Print($"Try to connect to {ip}");
			var address = IPAddress.Parse(ip);
			TcpClient tcpClient = new();
			await tcpClient.ConnectAsync(address, port);
			GD.Print($"Connect to {ip} succeeded or failed");
			
			connecting = false;
			return tcpClient;
		}

		public override async Task Tick()
		{
			await base.Tick();
			if (connecting) return;
			if (packets.Count == 0) return;

			await ProcessPacket(packets.Dequeue());
		}

		private static readonly Dictionary<string, System.Type> jsonTypes = new()
		{
			//game start
			{ Packet.GetDeck, typeof(GetDeckClientPacket)},
			{ Packet.DeckAccepted, typeof(DeckAcceptedClientPacket)},
			{ Packet.SetAvatar, typeof(SetAvatarClientPacket)},
			{ Packet.SetFirstTurnPlayer, typeof(SetFirstPlayerClientPacket)},
			//game end
			{ Packet.GameEnd, typeof(GameEndClientPacket)},
			//gamestate
			{ Packet.SetLeyload, typeof(SetLeyloadClientPacket)},
			{ Packet.SetTurnPlayer, typeof(SetTurnPlayerClientPacket)},
			{ Packet.PutCardsBack, typeof(PutCardsBackClientPacket)},
			{ Packet.AttackStarted, typeof(AttackStartedClientPacket)},
			{ Packet.HandSizeToStack, typeof(HandSizeToStackClientPacket)},
			{ Packet.ChooseHandSize, typeof(GetHandSizeChoicesClientPacket)},
			{ Packet.SetDeckCount, typeof(SetDeckCountClientPacket)},
			//card addition/deletion
			{ Packet.AddCard, typeof(AddCardClientPacket)},
			{ Packet.DeleteCard, typeof(DeleteCardClientPacket)},
			{ Packet.ChangeEnemyHandCount, typeof(ChangeEnemyHandCountClientPacket)},
			//card movement
			{ Packet.KnownToEnemy, typeof(UpdateKnownToEnemyClientPacket)},
			//public areas
			{ Packet.PlayCard, typeof(PlayCardClientPacket)},
			{ Packet.AttachCard, typeof(AttachCardClientPacket)},
			{ Packet.MoveCard, typeof(MoveCardClientPacket)},
			{ Packet.DiscardCard, typeof(DiscardCardClientPacket)},
			{ Packet.AnnihilateCard, typeof(AnnihilateCardClientPacket)},
			//private areas
			{ Packet.RehandCard, typeof(RehandCardClientPacket)},
			{ Packet.TopdeckCard, typeof(TopdeckCardClientPacket)},
			{ Packet.ReshuffleCard, typeof(ReshuffleCardClientPacket)},
			{ Packet.BottomdeckCard, typeof(BottomdeckCardClientPacket)},
			//stats
			{ Packet.UpdateCardNumericStats, typeof(ChangeCardNumericStatsClientPacket)},
			{ Packet.NegateCard, typeof(NegateCardClientPacket)},
			{ Packet.ActivateCard, typeof(ActivateCardClientPacket)},
			{ Packet.ChangeCardController, typeof(ChangeCardControllerClientPacket)},
			{ Packet.SetPips, typeof(SetPipsClientPacket)},
			{ Packet.AttacksThisTurn, typeof(AttacksThisTurnClientPacket)},
			{ Packet.SpacesMoved, typeof(SpacesMovedClientPacket)},
			//effects
			//targeting
			{ Packet.GetCardTarget, typeof(GetCardTargetClientPacket)},
			{ Packet.GetSpaceTarget, typeof(GetSpaceTargetClientPacket)},
			//other
			{ Packet.GetEffectOption, typeof(GetEffectOptionClientPacket)},
			{ Packet.EffectResolving, typeof(EffectResolvingClientPacket)},
			{ Packet.EffectActivated, typeof(EffectActivatedClientPacket)},
			{ Packet.RemoveStackEntry, typeof(RemoveStackEntryClientPacket)},
			{ Packet.SetEffectsX, typeof(SetEffectsXClientPacket)},
			{ Packet.PlayerChooseX, typeof(GetPlayerChooseXClientPacket)},
			{ Packet.TargetAccepted, typeof(TargetAcceptedClientPacket)},
			{ Packet.AddTarget, typeof(AddTargetClientPacket)},
			{ Packet.RemoveTarget, typeof(RemoveTargetClientPacket)},
			{ Packet.ToggleDecliningTarget, typeof(ToggleDecliningTargetClientPacket)},
			{ Packet.StackEmpty, typeof(StackEmptyClientPacket)},
			{ Packet.EffectImpossible, typeof(EffectImpossibleClientPacket)},
			{ Packet.OptionalTrigger, typeof(OptionalTriggerClientPacket)},
			{ Packet.ToggleAllowResponses, typeof(ToggleAllowResponsesClientPacket)},
			{ Packet.GetTriggerOrder, typeof(GetTriggerOrderClientPacket)},
			{ Packet.EditCardLink, typeof(EditCardLinkClientPacket)},
		}; //TODO a unit test that asserts that all types in this map extend IClientOrderPacket

		private static IClientOrderPacket? FromJson(string command, string json)
		{
			if (!jsonTypes.ContainsKey(command)) throw new System.ArgumentException($"Unrecognized command {command} in packet sent to client");

			return JsonConvert.DeserializeObject(json, jsonTypes[command], DeserializePacketJsonSettings) as IClientOrderPacket;
		}

		public override Task ProcessPacket((string command, string json) packetInfo)
		{
			if (packetInfo.command == Packet.Invalid)
			{
				GD.PrintErr("Invalid packet");
				return Task.CompletedTask;
			}

			var p = FromJson(packetInfo.command, packetInfo.json);
			GD.Print($"Parsing packet {packetInfo.command}");
			p?.Execute(game);

			//clean up any visual differences after the latest packet.
			//TODO make this more efficient, probably with dirty lists
			//game.Refresh();
			return Task.CompletedTask;
		}
	}
}