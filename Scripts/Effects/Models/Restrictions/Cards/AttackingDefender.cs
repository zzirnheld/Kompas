using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	/// <summary>
	/// A specialized AllOf containing the default elements, plus 
	/// </summary>
	public class AttackingDefender : AllOf, IAttackingDefender
	{
		[JsonProperty]
		public int maxPerTurn = 1;
		[JsonProperty]
		public bool waiveAdjacencyRequirement = false;

		protected override IEnumerable<IRestriction<IGameCardInfo>> DefaultElements
		{
			get
			{
				yield return new Gamestate.CardFitsRestriction()
				{
					card = new Identities.Cards.ThisCardNow(),
					cardRestriction = new AllOf()
					{
						elements = new IRestriction<IGameCardInfo>[]
						{
							new Friendly(),
							new Character(),
							new AtLocation(Location.Board)
						}
					}
				};

				yield return new Character();
				yield return new Enemy();

				if (!waiveAdjacencyRequirement)
				{
					yield return new Spaces.AdjacentTo()
					{
						card = new Identities.Cards.ThisCardNow()
					};
				}

				yield return new Gamestate.MaxPerTurn() { max = maxPerTurn, attacks = true };
				yield return new Gamestate.NothingHappening();
			}
		}
	}
}