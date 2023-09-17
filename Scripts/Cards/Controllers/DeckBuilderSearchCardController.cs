namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderSearchCardController : DeckBuilderCardController
	{
		public void AddToDeck() => DeckController.AddToDeck(Card);
	}
}