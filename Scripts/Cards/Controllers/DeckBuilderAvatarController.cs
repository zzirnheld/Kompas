using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderAvatarController : DeckBuilderDeckCardController
	{
		public void Clear() => InfoDisplayer.Clear();

		public void UpdateAvatar(DeckBuilderCard card) => Card = card;
	}
}