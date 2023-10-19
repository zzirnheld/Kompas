using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Views
{
	public class ClientTopLeftCardView : FocusableCardViewBase<ClientGameCard, ControlInfoDisplayer>
	{
		public ClientTopLeftCardView(ControlInfoDisplayer infoDisplayer)
			: base(infoDisplayer)
		{ }

		//Expose Focus and Show, since other inheritors don't expose them.
		//I'm realizing this is the opposite of what polymorphism is supposed to look like - not great.
		//TODO consider moving out the Focus logic to some other structure
		public new void Focus(ClientGameCard card) => base.Focus(card);
		public new void Show(ClientGameCard card, bool refresh = false) => base.Show(card, refresh);
	}
}