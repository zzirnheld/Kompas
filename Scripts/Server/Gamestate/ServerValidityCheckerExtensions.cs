using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Models;

namespace Kompas.Server.Gamestate.Extensions
{
	public static class ServerValidityCheckerExtensions
	{
		public static bool IsValidNormalPlay(this ServerGame game, ServerGameCard card, Space to, IPlayer player)
		{
			if (card == null) return false;
			if (game.DebugMode)
			{
				GD.PushWarning("Debug mode, always return true for valid play");
				return true;
			}

			//Debug.Log($"Checking validity of playing {card.CardName} to {to}");
			return card.PlayRestriction.IsValid((to, player), ResolutionContext.PlayerTrigger(null, game));
		}

		public static bool IsValidNormalAttach(this ServerGame game, ServerGameCard card, Space to, IPlayer player)
		{
			if (game.DebugMode)
			{
				GD.PushWarning("Debug mode, always return true for valid augment");
				return true;
			}

			//Debug.Log($"Checking validity augment of {card.CardName} to {to}, on {boardCtrl.GetCardAt(to)}");
			return card != null && card.Type == 'A' && to.IsValid
				&& !game.Board.IsEmpty(to)
				&& card.PlayRestriction.IsValid((to, player), ResolutionContext.PlayerTrigger(null, game));
		}

		public static bool IsValidNormalMove(this ServerGame game, ServerGameCard toMove, Space to, IPlayer by)
		{
			if (game.DebugMode)
			{
				GD.PushWarning("Debug mode, always return true for valid move");
				return true;
			}

			//Debug.Log($"Checking validity of moving {toMove.CardName} to {to}");
			if (toMove.Position == to) return false;
			else return toMove.MovementRestriction.IsValid(to, ResolutionContext.PlayerTrigger(null, game));
		}

		public static bool IsValidNormalAttack(this ServerGame game, ServerGameCard attacker, ServerGameCard defender, IPlayer instigator)
		{
			if (game.DebugMode)
			{
				GD.PushWarning("Debug mode, always return true for valid attack");
				return attacker != null && defender != null;
			}

			//Debug.Log($"Checking validity of attack of {attacker.CardName} on {defender} by {instigator.index}");
			return attacker.AttackingDefenderRestriction.IsValid(defender, context: ResolutionContext.PlayerTrigger(null, game));
		}
	}
}