using Godot;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Gamestate;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Gamestate
{
	public partial class ClientTargetingController : Node
	{
		[Export]
		private ControlInfoDisplayer TopLeftInfoDisplayer { get; set; }

		public ClientTopLeftCardView TopLeftCardView { get; private set; }

		/// <summary>
        /// The view already contains the logic for focusing on a given card, for entirely historical reasons.
        /// I could rip that out and move it here, but why bother
        /// </summary>
		public ClientGameCard FocusedCard => TopLeftCardView.FocusedCard;
		public ClientGameCard ShownCard => TopLeftCardView.ShownCard;

		public override void _Ready()
		{
			base._Ready();
			TopLeftCardView = new(TopLeftInfoDisplayer);
		}

		/// <summary>
        /// If applicable, tries to play the current FocusedCard to <paramref name="space"/>
        /// </summary>
		public void Select(Space space)
		{
			//TODO make client notifier a static helper class
			GD.Print($"Selecting {space}");
			FocusedCard?.ClientGame.ClientGameController.Notifier.RequestPlay(FocusedCard, space.x, space.y);
		}

		public void Select(ClientGameCard card)
		{
			GD.Print($"Selecting {card}");
			TopLeftCardView.Focus(card);
		}

		public void Highlight(ClientGameCard card)
		{
			//GD.Print($"Selecting {card}");
			TopLeftCardView.Show(card);
		}
	}
}