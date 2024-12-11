using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views;

public class DeckBuilderTopLeftCardView : TopLeftCardViewBase<DeckBuilderCard>
{
	protected override CardRepository CardRepository { get; }

	public DeckBuilderTopLeftCardView(ControlInfoDisplayer infoDisplayer, ReminderTextPopup reminderTextPopup,
		CardRepository cardRepository)
		: base(infoDisplayer, reminderTextPopup)
	{
		Logger.Log("Creating dbtlcv");
		CardRepository = cardRepository;
	}

	public void Show(DeckBuilderCard? card) => base.Show(card);
}