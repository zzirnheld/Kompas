
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.UI.GameStart;

namespace Kompas.Cards.Controllers
{
	/// <summary>
    /// Extends TextureRect because the TextureRect has to be the parent, otherwise the grid doesn't line up.
    /// For some reason.
    /// </summary>
	public partial class SelectDeckCardController : Control
	{
		[Export]
		public SelectDeckInfoDisplayer? InfoDisplayer { get; private set; }

		private CardBase? _card;
		public CardBase? Card
		{
			get => _card;
			protected set
			{
				_card = value;
				MyView.Show(_card);
			}
		}

		private SelectDeckCardView? _myView;
		private SelectDeckCardView MyView => _myView ??= new SelectDeckCardView(InfoDisplayer
			?? throw new System.NullReferenceException("Forgot to init"));

		public void Init(CardBase card)
		{
			Card = card;
		}
	}
}