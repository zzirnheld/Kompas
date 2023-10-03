using System.Collections.Generic;
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
						elements = new IRestriction<IGameCard>[] {
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