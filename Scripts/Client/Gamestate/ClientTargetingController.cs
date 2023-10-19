using Godot;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
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
		public ClientGameCard FocusedCard
		{
			get => TopLeftCardView.FocusedCard;
			set => TopLeftCardView.Focus(value);
		}

		public ClientGameCard ShownCard
		{
			get => TopLeftCardView.ShownCard;
			set => TopLeftCardView.Show(value);
		}

		public override void _Ready()
		{
			base._Ready();
			TopLeftCardView = new(TopLeftInfoDisplayer);
		}
	}
}