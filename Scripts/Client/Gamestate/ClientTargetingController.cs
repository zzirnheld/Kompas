using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Gamestate
{
	public enum TargetMode { Free, OnHold, CardTarget, CardTargetList, SpaceTarget, HandSize }

	public partial class ClientTargetingController : Node
	{
		[Export]
		private ControlInfoDisplayer TopLeftInfoDisplayer { get; set; }
		[Export]
		private ClientGameController GameController { get; set; }

		public ClientTopLeftCardView TopLeftCardView { get; private set; }

		/// <summary>
        /// The view already contains the logic for focusing on a given card, for entirely historical reasons.
        /// I could rip that out and move it here, but why bother
        /// </summary>
		public ClientGameCard FocusedCard => TopLeftCardView.FocusedCard;
		public ClientGameCard ShownCard => TopLeftCardView.ShownCard;

		private ClientSearch clientSearch;

		public TargetMode TargetMode { get; private set; }

		public override void _Ready()
		{
			base._Ready();
			TopLeftCardView = new(TopLeftInfoDisplayer);
			TopLeftCardView.FocusChange += (_, change) => FocusChange(change.Old, change.New);
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
			clientSearch?.ToggleTarget(card);
		}

		public void Highlight(ClientGameCard card)
		{
			//GD.Print($"Selecting {card}");
			TopLeftCardView.Show(card);
		}

		private static void FocusChange(ClientGameCard old, ClientGameCard current)
		{
			old?.ClientCardController.ShowFocused(false);
			current.ClientCardController.ShowFocused(true);
		}

		public void StartSearch(TargetMode targetMode, IEnumerable<int> potentialTargetIDs, IListRestriction listRestriction)
		{
			TargetMode = targetMode;
			clientSearch = ClientSearch.StartSearch(potentialTargetIDs.Select(GameController.Game.LookupCardByID), listRestriction,
				GameController.Game, this, GameController.Notifier);
		}

		public void FinishSearch()
		{
			TargetMode = TargetMode.OnHold;
			clientSearch = null;
		}
	}
}