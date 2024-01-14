using Newtonsoft.Json;

namespace Kompas.Networking
{
	//not abstract so can deserialize to it
	public class Packet
	{
		//TODO replace these all with just the type of the packet?
		//the sending packet constructor would need to have the associated
		//string of the type of the server packet.
		//would that be better?
		#region commands
		public const string Invalid = "Invalid Command";

		//game start
		public const string GetDeck = "Get Deck";
		public const string SetDeck = "Set Deck";
		public const string DeckAccepted = "Deck Accepted";
		public const string SetAvatar = "Set Avatar";
		public const string SetFirstTurnPlayer = "Set First Turn Player";
		//game end
		public const string GameEnd = "Game End";

		//player action: things the player initiates (client to server)
		public const string PlayAction = "Player Play Action";
		public const string AugmentAction = "Player Augment Action";
		public const string MoveAction = "Player Move Action";
		public const string AttackAction = "Player Attack Action";
		public const string EndTurnAction = "Player End Turn Action";
		public const string ActivateEffectAction = "Player Activate Effect Action";

		//effect commands
		//from client to server
		public const string CardTargetChosen = "Card Target Chosen";
		public const string SpaceTargetChosen = "Space Target Chosen";
		public const string XSelectionChosen = "X Value Chosen";
		public const string DeclineAnotherTarget = "Decline Another Target";
		public const string ListChoicesChosen = "List Choices Chosen";
		public const string OptionalTriggerResponse = "Optional Trigger Answered";
		public const string ChooseEffectOption = "Choose Effect Option";
		public const string PassPriority = "Pass Priority";
		public const string ChooseTriggerOrder = "Choose Trigger Order";
		//from server to client
		//targeting
		public const string GetCardTarget = "Get Card Target";
		public const string GetListChoices = "Get List Choices";
		public const string GetSpaceTarget = "Get Space Target";
		public const string EditCardLink = "Edit Card Link";
		//other effect
		public const string GetEffectOption = "Get Effect Option";
		public const string EffectResolving = "Effect Resolving";
		public const string EffectActivated = "Effect Activated";
		public const string RemoveStackEntry = "Remove Stack Entry";
		public const string SetEffectsX = "Set Effects X";
		public const string PlayerChooseX = "Player Choose X";
		public const string TargetAccepted = "Target Accepted";
		public const string AddTarget = "Add Target";
		public const string RemoveTarget = "Remove Target";
		public const string ToggleDecliningTarget = "Toggle Declining Target";
		public const string DiscardSimples = "Discard Simples";
		public const string StackEmpty = "Stack Empty";
		public const string EffectImpossible = "Effect Impossible";
		public const string OptionalTrigger = "Optional Trigger";
		public const string ToggleAllowResponses = "Toggle Allow Responses";
		public const string GetTriggerOrder = "Get Trigger Order";

		//gamestate (from server to client)
		public const string SetLeyload = "Set Leyload";
		public const string SetTurnPlayer = "Set Turn Player";
		public const string AttackStarted = "Attack Started";
		public const string ChooseHandSize = "Choose Hand Size";
		public const string HandSizeToStack = "Hand Size to Stack";
		public const string SetDeckCount = "Set Deck Count";
		//gamestate (from client to server)
		public const string HandSizeChoices = "Hand Size Choices";

		//card addition/deletion (from server to client)
		public const string DeleteCard = "Delete Card";
		public const string AddCard = "Add Card";
		public const string ChangeEnemyHandCount = "Change Enemy Hand Count";

		//card movement (from server to client)
		public const string PutCardsBack = "Put Cards Back";
		public const string KnownToEnemy = "Update Known To Enemy";
		//public locations
		public const string PlayCard = "Play Card";
		public const string AttachCard = "Attach Card";
		public const string MoveCard = "Move Card";
		public const string DiscardCard = "Discard Card";
		public const string AnnihilateCard = "Annihilate Card";
		//private locations
		public const string RehandCard = "Rehand Card";
		public const string TopdeckCard = "Topdeck Card";
		public const string ReshuffleCard = "Reshuffle Card";
		public const string BottomdeckCard = "Bottomdeck Card";

		//stats
		public const string UpdateCardNumericStats = "Change Card Numeric Stats";
		public const string NegateCard = "Negate Card";
		public const string ActivateCard = "Activate Card";
		public const string ResetCard = "Reset Card";
		public const string ChangeCardController = "Change Card Controller";
		public const string SetPips = "Set Pips";
		public const string SpacesMoved = "Spaces Moved";
		public const string AttacksThisTurn = "Attacks This Turn";

		//debug commands
		//from client to server
		public const string DebugTopdeck = "DEBUG COMMAND Topdeck";
		public const string DebugDiscard = "DEBUG COMMAND Discard";
		public const string DebugRehand = "DEBUG COMMAND Rehand";
		public const string DebugDraw = "DEBUG COMMAND Draw";
		public const string DebugSetNESW = "DEBUG COMMAND Set NESW";
		public const string DebugSetPips = "DEBUG COMMAND Set Pips";
		#endregion commands

		/// <summary>
		/// Contains the command that is sent.
		/// Default is invalid to raise a problem if a sent packet has no command.
		/// </summary>
		public string command = Invalid;

		//Json serializer needs a parameterless constructor
		public Packet() { }

		public Packet(string command)
		{
			this.command = command;
		}

		/// <summary>
		/// Creates an exact copy of this packet to send.
		/// </summary>
		/// <returns></returns>
		public virtual Packet Copy() => new(command);

		/// <summary>
		/// Creates a version of this packet that the opposite player will understand.
		/// </summary>
		/// <returns></returns>
		public virtual Packet? GetInversion(bool known = true) => known ? Copy() : null;

		public override string ToString() => JsonConvert.SerializeObject(this);
	}
}