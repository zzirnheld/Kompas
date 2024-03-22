using System;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Networking;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate.Extensions;
using Kompas.Server.Gamestate.Locations.Models;
using Kompas.Server.Networking;
using Kompas.Shared.Exceptions;

namespace Kompas.Server.Gamestate.Players
{
	public class ServerPlayer : IPlayer
	{
		private ServerNetworker? _networker;
		public ServerNetworker Networker => _networker
			?? throw new UseFactoryException();
		Networker IPlayer.Networker => Networker;

		public ServerGame ServerGame { get; }
		public IGame Game => ServerGame;

		private IPlayer? _enemy;
		public IPlayer Enemy => _enemy
			?? throw new UseFactoryException();

		private int _pips;
		public int Pips
		{
			get => _pips;
			set
			{
				_pips = value;
				PlayerController.Pips = value;
				ServerNotifier.NotifySetPips(this, value);
			}
		}
		public int PipsNextTurn { set { } }

		private GameCard? _avatar;
		public GameCard Avatar
		{
			get => _avatar ?? throw new NotInitializedException();
			set
			{
				if (_avatar != null)
					throw new System.InvalidOperationException($"Tried to set {Index}'s avatar to {value} when it was already {_avatar}!");

				_avatar = value;
			}
		}

		public bool Friendly => false; //no such thing in the hostile lands of "the server"

		public int Index { get; }

		private IDeck<ServerGameCard>? _deck;
		public IDeck<ServerGameCard> Deck => _deck
			?? throw new UseFactoryException();
		IDeck IPlayer.Deck => Deck;

		private IDiscard<ServerGameCard>? _discard;
		public IDiscard<ServerGameCard> Discard => _discard
			?? throw new UseFactoryException();
		IDiscard IPlayer.Discard => Discard;

		private IHand<ServerGameCard>? _hand;
		public IHand<ServerGameCard> Hand => _hand
			?? throw new UseFactoryException();
		IHand IPlayer.Hand => Hand;

		private IAnnihilation<ServerGameCard>? _annihilation;
		public IAnnihilation<ServerGameCard> Annihilation => _annihilation
			?? throw new UseFactoryException();
		IAnnihilation IPlayer.Annihilation => Annihilation;

		public Space AvatarCorner => Index == 0 ? Space.NearCorner : Space.FarCorner;

		public PlayerController PlayerController { get; }

		private ServerPlayer(ServerGame game, int index, PlayerController playerController)
		{
			ServerGame = game;
			Index = index;
			PlayerController = playerController;
		}

		//Factory methods, so we can initialize the location models with the player
		private static ServerPlayer Create(ServerGame game, PlayerController controller, int index, GetNetworker getNetworker)
		{
			ServerPlayer ret = new(game, index, controller);

			ret._deck = new ServerDeck(ret, controller.DeckController, game);
			ret._discard = new ServerDiscard(ret, controller.DiscardController, game);
			ret._hand = new ServerHand(ret, controller.HandController, game);
			ret._annihilation = new ServerAnnihilation(ret, controller.AnnihilationController, game);
			ret._networker = getNetworker(ret, index);

			return ret;
		}

		public static ServerPlayer[] Create(ServerGameController gameController, GetNetworker getNetworker)
		{
			ServerPlayer[] ret =
			{
				Create(gameController.ServerGame, gameController.PlayerControllers[0], 0, getNetworker),
				Create(gameController.ServerGame, gameController.PlayerControllers[1], 1, getNetworker),
			};

			ret[0]._enemy = ret[1];
			ret[1]._enemy = ret[0];

			return ret;
		}

		public delegate ServerNetworker GetNetworker(ServerPlayer player, int index);


		//If the player tries to do something, it goes here to check if it's ok, then do it if it is ok.
		#region IPlayer Control Methods
		/// <summary>
		/// x and y here are from playerIndex's perspective
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public async Task TryAugment(GameCard? aug, Space? space)
		{
			if (aug == null || space == null) return;
			try
			{
				if (ServerGame.IsValidNormalAttach(aug, space, this))
				{
					aug.Play(space, this, payCost: true);
					await ServerGame.StackController.CheckForResponse();
				}
				else ServerNotifier.NotifyPutBack(this);
			}
			catch (KompasException ke)
			{
				GD.PrintErr(ke);
				ServerNotifier.NotifyPutBack(this);
			}
		}

		public async Task TryPlay(GameCard? card, Space? space)
		{
			if (card == null || space == null) return;
			try
			{
				if (ServerGame.IsValidNormalPlay(card, space, this))
				{
					card.Play(space, this, payCost: true);
					await ServerGame.StackController.CheckForResponse();
				}
				else
				{
					GD.PushWarning($"Player {Index} attempted an invalid play of {card} to {space}.");
					ServerNotifier.NotifyPutBack(this);
				}
			}
			catch (KompasException ke)
			{
				GD.PrintErr($"Player {Index} attempted an invalid play of {card} to {space}. Resulting exception:\n{ke}");
				ServerNotifier.NotifyPutBack(this);
			}
		}

		public async Task TryMove(GameCard? toMove, Space? space)
		{
			if (toMove == null || space == null) return;
			//if it's not a valid place to do, put the cards back
			try
			{
				if (ServerGame.IsValidNormalMove(toMove, space, this))
				{
					toMove.Move(space, true, this);
					await ServerGame.StackController.CheckForResponse();
				}
				else ServerNotifier.NotifyPutBack(this);
			}
			catch (KompasException ke)
			{
				GD.PrintErr(ke);
				ServerNotifier.NotifyPutBack(this);
			}
		}

		/// <summary>
		/// If it is a valid action to take, activates the effect, adding it to the stack and suchlike
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="controller"></param>
		public async Task TryActivateEffect(ServerEffect? effect)
		{
			GD.Print($"Player {Index} trying to activate effect of {effect?.Card?.CardName}");
			if (effect != null && effect.CanBeActivatedBy(this))
			{
				var context = ServerResolutionContext.PlayerTrigger(effect, Game, this);
				ServerGame.StackController.PushToStack(effect, this, context);
				await ServerGame.StackController.CheckForResponse();
			}
		}

		public async Task TryAttack(GameCard? attacker, GameCard? defender)
		{
			if (attacker == null || defender == null) return;
			ServerNotifier.NotifyBothPutBack(new IPlayer[] {this, Enemy});

			if (ServerGame.IsValidNormalAttack(attacker, defender, this))
			{
				ServerGame.Attack(attacker, defender, this, stackSrc: default, manual: true);
				await ServerGame.StackController.CheckForResponse();
			}
		}

		public async Task TryEndTurn()
		{
			if (Game.StackController.NothingHappening && ServerGame.TurnPlayer == this)
				await ServerGame.SwitchTurn();
		}
		#endregion IPlayer Control Methods
	}
}