using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Networking;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate.Extensions;
using Kompas.Server.Gamestate.Locations.Models;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Players
{
	public class ServerPlayer : IPlayer
	{
		//TODO encapsulate
		public ServerNetworker Networker { get; private set; }
		Networker IPlayer.Networker => Networker;

		public ServerGame ServerGame { get; }
		public IGame Game => ServerGame;

		public IPlayer Enemy { get; private set; }

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

		private GameCard _avatar;
		public GameCard Avatar
		{
			get => _avatar;
			set
			{
				if (_avatar != null)
					throw new System.InvalidOperationException($"Tried to set {Index}'s avatar to {value} when it was already {_avatar}!");

				_avatar = value;
			}
		}

		public bool Friendly => false; //no such thing in the hostile lands of "the server"

		public int Index { get; }

		public Deck Deck { get; private set; }
		public Discard Discard { get; private set; }
		public Hand Hand { get; private set; }
		public Annihilation Annihilation { get; private set; }

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

			ret.Deck = new ServerDeck(ret, controller.DeckController, game);
			ret.Discard = new ServerDiscard(ret, controller.DiscardController, game);
			ret.Hand = new ServerHand(ret, controller.HandController, game);
			ret.Annihilation = new ServerAnnihilation(ret, controller.AnnihilationController, game);
			ret.Networker = getNetworker(ret, index);

			return ret;
		}

		public static ServerPlayer[] Create(ServerGameController gameController, GetNetworker getNetworker)
		{
			ServerPlayer[] ret =
			{
				Create(gameController.ServerGame, gameController.PlayerControllers[0], 0, getNetworker),
				Create(gameController.ServerGame, gameController.PlayerControllers[1], 1, getNetworker),
			};

			ret[0].Enemy = ret[1];
			ret[1].Enemy = ret[0];

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
		public async Task TryAugment(GameCard aug, Space space)
		{
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

		public async Task TryPlay(GameCard card, Space space)
		{
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

		public async Task TryMove(GameCard toMove, Space space)
		{
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
		public async Task TryActivateEffect(ServerEffect effect)
		{
			GD.Print($"Player {Index} trying to activate effect of {effect?.Card?.CardName}");
			if (effect.CanBeActivatedBy(this))
			{
				var context = ServerResolutionContext.PlayerTrigger(effect, Game, this);
				ServerGame.StackController.PushToStack(effect, this, context);
				await ServerGame.StackController.CheckForResponse();
			}
		}

		public async Task TryAttack(GameCard attacker, GameCard defender)
		{
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