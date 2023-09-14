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

		protected override IEnumerable<IRestriction<GameCardBase>> DefaultElements
		{
			get
			{
				yield return new Restrictions.Gamestate.CardFitsRestriction()
				{
					card = new Identities.Cards.ThisCardNow(),
					cardRestriction = new AllOf()
					{
						elements = new IRestriction<GameCardBase>[]
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
					yield return new Restrictions.Spaces.AdjacentTo()
					{
						card = new Identities.Cards.ThisCardNow()
					};
				}

				yield return new Restrictions.Gamestate.MaxPerTurn() { max = maxPerTurn };
				yield return new Restrictions.Gamestate.NothingHappening();
			}
		}
	}
}