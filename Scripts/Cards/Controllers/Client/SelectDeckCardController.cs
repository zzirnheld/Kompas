
using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Views.Client;
using Kompas.UI.Client.GameStart;

namespace Kompas.Cards.Controllers.Client
{
	/// <summary>
	/// Extends TextureRect because the TextureRect has to be the parent, otherwise the grid doesn't line up.
	/// For some reason.
	/// </summary>
	public partial class SelectDeckCardController : Control
	{
		[Export]
		public SelectDeckInfoDisplayer InfoDisplayer { get; private set; }

		private CardBase card;
		public CardBase Card
		{
			get => card;
			protected set
			{
				card = value;
				MyView.Show(card);
			}
		}

		private SelectDeckCardView _myView;
		private SelectDeckCardView MyView => _myView ??= new SelectDeckCardView(InfoDisplayer);

		public void Init(CardBase card)
		{
			Card = card;
		}
	}
}