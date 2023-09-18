using Kompas.Cards.Models;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderAvatarController : DeckBuilderDeckCardController
	{
		public void Clear()
		{
			InfoDisplayer.Clear();
			Card = null;
		}

		public void UpdateAvatar(DeckBuilderCard card) => Card = card;
	}
}