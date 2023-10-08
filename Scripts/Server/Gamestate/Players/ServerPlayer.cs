using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Models;
using Kompas.Server.Networking;

namespace Kompas.Server.Gamestate.Players
{
	public class ServerPlayer : IPlayer
	{
		public ServerNotifier notifier;

		public IGame Game => throw new System.NotImplementedException();

		public IPlayer Enemy => throw new System.NotImplementedException();

		public int Pips { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public GameCard Avatar { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public bool Friendly => throw new System.NotImplementedException();

		public int Index => throw new System.NotImplementedException();

		public Deck Deck => throw new System.NotImplementedException();

		public Discard Discard => throw new System.NotImplementedException();

		public Hand Hand => throw new System.NotImplementedException();

		public Annihilation Annihilation => throw new System.NotImplementedException();


		//If the player tries to do something, it goes here to check if it's ok, then do it if it is ok.
		#region Player Control Methods
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
				Debug.LogError(ke);
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
					Debug.LogWarning($"Player {index} attempted an invalid play of {card} to {space}.");
					notifier.NotifyPutBack();
				}
			}
			catch (KompasException ke)
			{
				Debug.LogError($"Player {index} attempted an invalid play of {card} to {space}. Resulting exception:\n{ke}");
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
				Debug.LogError(ke);
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
			Debug.Log($"Player {index} trying to activate effect of {effect?.Source?.CardName}");
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
		#endregion Player Control Methods
	}
}