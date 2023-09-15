using Kompas.Cards.Models;

namespace Kompas.UI.CardInformationDisplayers
{
	/// <summary>
    /// Defines how information is actually displayed on a card.
    /// </summary>
	public interface ICardInfoDisplayer
	{
		/// <summary>
		/// Shows or hides the card information.
		/// Can override for special behavior, but the default is to just enable/disable the GameObject
		/// </summary>
		public bool ShowingInfo { set; }

		/// <summary>
		/// Display the ShownCard's rules text, like its name, type line, and effect text.
		/// Called only when the card's info changes, or is refreshed
		/// </summary>
		public void DisplayCardRulesText(CardBase card);

		/// <summary>
		/// Display the ShownCard's stats
		/// Called only when the card's info changes, or is refreshed
		/// </summary>
		public void DisplayCardNumericStats(CardBase card);

		/// <summary>
		/// Display the ShownCard's image, as appropriate
		/// Called only when the card's info changes, or is refreshed
		/// </summary>
		public void DisplayCardImage(CardBase card);
	}
}