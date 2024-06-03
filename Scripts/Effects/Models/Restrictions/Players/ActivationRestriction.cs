using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Players
{
	public class ActivationRestriction : AllOf, IActivationRestriction
	{
		[JsonProperty]
		public string[] locations = { Location.Board.StringVersion() };

		//I don't love doing casting, but it feels viscerally wrong to put a max per turn/round/stack as a property on IRestriction
		public int? MaxUsesPerTurn
		{
			get
			{
				foreach (var elem in elements)
				{
					if (elem is Gamestate.MaxPerTurn max) return max.max;
				}
				return null;
			}
		}

		public int? MaxUsesPerRound
		{
			get
			{
				foreach (var elem in elements)
				{
					if (elem is Gamestate.MaxPerRound max) return max.max;
				}
				return null;
			}
		}

		public int? MaxUsesPerStack
		{
			get
			{
				foreach (var elem in elements)
				{
					if (elem is Gamestate.MaxPerStack max) return max.max;
				}
				return null;
			}
		}

		protected override IEnumerable<IRestriction<IPlayer>> DefaultElements
		{
			get
			{
				yield return new Gamestate.FriendlyTurn();
				yield return new Gamestate.CardFitsRestriction()
				{
					card = new Identities.Cards.ThisCardNow(),
					cardRestriction = new Cards.AllOf()
					{
						elements = new IRestriction<IGameCardInfo>[] {
							new Cards.AtLocation() { locations = this.locations },
							new Cards.Not() { negated = new Cards.Negated() }
						}
					}
				};
				yield return new Gamestate.NothingHappening();
				yield return new Gamestate.Not() { negated = new Gamestate.EffectAlreadyTriggered() };
				yield return new Is() { player = new Identities.Players.FriendlyPlayer() };
			}
		}
	}
}