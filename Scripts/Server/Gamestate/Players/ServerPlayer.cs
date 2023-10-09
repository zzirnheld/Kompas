using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate.Locations.Controllers;
using Kompas.Server.Gamestate.Locations.Models;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Players
{
	public class ServerPlayer : IPlayer
	{
		//TODO encapsulate
		public ServerNetworker Networker { get; init; }

		public IGame Game { get; }

		public IPlayer Enemy { get; private set; }

		public int Pips { get; set; }

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

		private ServerPlayer(ServerGame game, int index)
		{
			Game = game;
			Index = index;
		}

		//Factory methods, so we can initialize the location models with the player
		private static ServerPlayer Create(ServerGame game, PlayerController controller, int index)
		{
			ServerPlayer ret = new(game, index);

			ret.Deck = new ServerDeck(ret, controller.DeckController, game);
			ret.Discard = new ServerDiscard(ret, controller.DiscardController, game);
			ret.Hand = new ServerHand(ret, controller.HandController, game);
			ret.Annihilation = new ServerAnnihilation(ret, controller.AnnihilationController, game);

			return ret;
		}

		public static ServerPlayer[] Create(ServerGameController gameController)
		{
			ServerPlayer[] ret =
			{
				Create(gameController.ServerGame, gameController.PlayerControllers[0], 0),
				Create(gameController.ServerGame, gameController.PlayerControllers[1], 1),
			};

			ret[0].Enemy = ret[1];
			ret[1].Enemy = ret[0];

			return ret;
		}


		//If the player tries to do something, it goes here to check if it's ok, then do it if it is ok.
		#region IPlayer Control Methods
		/// <summary>
		/// x and y here are from playerIndex's perspective
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public async Task TryAugment(GameCard aug, Space space)
		{
			throw new System.NotImplementedException();
			/*
			try
			{
				if (Game.IsValidNormalAttach(aug, space, this))
				{
					aug.Play(space, this, payCost: true);
					await game.effectsController.CheckForResponse();
				}
				else notifier.NotifyPutBack();
			}
			catch (KompasException ke)
			{
				GD.PrintErr(ke);
				notifier.NotifyPutBack();
			}*/
		}

		public async Task TryPlay(GameCard card, Space space)
		{
			throw new System.NotImplementedException();
			/*
			try
			{
				if (game.IsValidNormalPlay(card, space, this))
				{
					card.Play(space, this, payCost: true);
					await game.effectsController.CheckForResponse();
				}
				else
				{
					GD.PushWarning($"Player {index} attempted an invalid play of {card} to {space}.");
					notifier.NotifyPutBack();
				}
			}
			catch (KompasException ke)
			{
				GD.PrintErr($"Player {index} attempted an invalid play of {card} to {space}. Resulting exception:\n{ke}");
				notifier.NotifyPutBack();
			}*/
		}

		public async Task TryMove(GameCard toMove, Space space)
		{
			throw new System.NotImplementedException();
			/*
			//if it's not a valid place to do, put the cards back
			try
			{
				if (game.IsValidNormalMove(toMove, space, this))
				{
					toMove.Move(space, true);
					await game.effectsController.CheckForResponse();
				}
				else notifier.NotifyPutBack();
			}
			catch (KompasException ke)
			{
				GD.PrintErr(ke);
				notifier.NotifyPutBack();
			}*/
		}

		/// <summary>
		/// If it is a valid action to take, activates the effect, adding it to the stack and suchlike
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="controller"></param>
		public async Task TryActivateEffect(ServerEffect effect)
		{
			throw new System.NotImplementedException();
			/*
			GD.Print($"Player {index} trying to activate effect of {effect?.Card?.CardName}");
			if (effect.CanBeActivatedBy(this))
			{
				var context = ResolutionContext.PlayerTrigger(effect, game);
				game.effectsController.PushToStack(effect, this, context);
				await game.effectsController.CheckForResponse();
			}
			*/
		}

		public async Task TryAttack(GameCard attacker, GameCard defender)
		{
			throw new System.NotImplementedException();
			/*
			notifier.NotifyBothPutBack();

			if (game.IsValidNormalAttack(attacker, defender, this))
			{
				game.Attack(attacker, defender, this, stackSrc: default, manual: true);
				await game.effectsController.CheckForResponse();
			}*/
		}

		public async Task TryEndTurn()
		{
			throw new System.NotImplementedException();
			/*
			if (game.NothingHappening && game.TurnPlayer == this)
				await game.SwitchTurn();
				*/
		}
		#endregion IPlayer Control Methods
	}
}