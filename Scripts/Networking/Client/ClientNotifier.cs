using Kompas.Networking;

namespace Kompas.Client.Networking
{
	public class ClientNotifier
	{
		private readonly ClientNetworker networkController;

		public ClientNotifier(ClientNetworker networkController)
		{
			this.networkController = networkController;
		}

		private void Send(Packet packet)
		{
			//if (packet != null) Debug.Log($"Sending packet {packet}");
			networkController.SendPacket(packet);
		}

		/*
		#region Normal Request Actions
		public void RequestPlay(GameCard card, int toX, int toY)
		{
			Debug.Log($"Requesting to play {card} to {toX}, {toY}");
			if (card.CardType == 'A') Send(new AugmentActionPacket(card.ID, toX, toY));
			else Send(new PlayActionPacket(card.ID, toX, toY));
		}

		public void RequestMove(GameCard card, int toX, int toY)
			=> Send(new MoveActionPacket(card.ID, toX, toY));

		public void RequestAttack(GameCard attacker, GameCard defender)
			=> Send(new AttackActionPacket(attacker.ID, defender.ID));

		public void RequestDecklistImport(string decklist)
			=> Send(new SetDeckPacket(decklist));

		public void RequestEndTurn() => Send(new EndTurnActionPacket());

		public void RequestTarget(GameCard card) => Send(new CardTargetPacket(card.ID));

		public void RequestActivateEffect(GameCard card, int index)
			=> Send(new ActivateEffectActionPacket(card.ID, index));

		public void RequestSetX(int x) => Send(new SelectXPacket(x));

		public void DeclineAnotherTarget() => Send(new DeclineAnotherTargetPacket());

		public void RequestSpaceTarget(int x, int y) => Send(new SpaceTargetPacket(x, y));

		public void RequestListChoices(IEnumerable<GameCard> choices) => Send(new ListChoicesPacket(choices));

		public void RequestHandSizeChoices(int[] cardIds) => Send(new SendHandSizeChoicesPacket(cardIds));

		public void RequestTriggerReponse(bool answer) => Send(new OptionalTriggerAnswerPacket(answer));

		public void RequestChooseEffectOption(int option) => Send(new EffectOptionResponsePacket(option));

		public void ChooseTriggerOrder(IEnumerable<(Trigger, int)> triggers)
		{
			int count = triggers.Count();
			int[] cardIds = new int[count];
			int[] effIndices = new int[count];
			int[] orders = new int[count];
			int i = 0;
			foreach (var (t, o) in triggers)
			{
				cardIds[i] = t.Source.ID;
				effIndices[i] = t.Effect.EffectIndex;
				orders[i] = o;
				i++;
			}
			Send(new TriggerOrderResponsePacket(cardIds, effIndices, orders));
		}

		public void DeclineResponse() => Send(new PassPriorityPacket());
		#endregion
		*/
	}
}