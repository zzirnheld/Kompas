using Kompas.Cards.Models;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderAvatarController : DeckBuilderDeckCardController
	{
		public void Clear()
		{
			_ = InfoDisplayer ?? throw new System.NullReferenceException("Forgot to init");
			InfoDisplayer.Clear();
			Card = null;
		}

		public void UpdateAvatar(DeckBuilderCard card) => Card = card;
	}
}