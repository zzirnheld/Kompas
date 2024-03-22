using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Client.Networking;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Search
{
	public class HandSizeSearch : CardSearch
	{
		public HandSizeSearch(IEnumerable<IGameCard> toSearch, IListRestriction listRestriction,
			IGame game, ClientTargetingController targetingController, ClientNotifier clientNotifier)
			: base(toSearch, listRestriction, game, targetingController, clientNotifier)
		{ }

		protected override void SendChoices(IList<IGameCard> choices)
			=> clientNotifier.RequestHandSizeChoices(choices.Select(c => c.ID).ToArray());
	}
}