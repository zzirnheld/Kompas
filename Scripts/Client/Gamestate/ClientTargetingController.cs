using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.Gamestate.Search;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Gamestate
{
	public partial class ClientTargetingController : Node
	{
		[Export]
		private ControlInfoDisplayer? TopLeftInfoDisplayer { get; set; }
		[Export]
		private ClientGameController? GameController { get; set; }
		[Export]
		private Control? CanDeclineFurtherTargetsButton { get; set; }

		public ClientTopLeftCardView? TopLeftCardView { get; private set; }

		/// <summary>
        /// The view already contains the logic for focusing on a given card, for entirely historical reasons.
        /// I could rip that out and move it here, but why bother
        /// </summary>
		public ClientGameCard? FocusedCard => TopLeftCardView?.FocusedCard;
		public ClientGameCard? ShownCard => TopLeftCardView?.ShownCard;

		private ISearch? currentSearch;

		public bool CanDeclineFurtherTargets
		{
			set
			{
				if (CanDeclineFurtherTargetsButton == null) throw new System.NullReferenceException("Forgot to init");
				CanDeclineFurtherTargetsButton.Visible = value;
			}
		}

		public override void _Ready()
		{
			base._Ready();
			if (TopLeftInfoDisplayer == null) throw new System.NullReferenceException("Forgot to init");
			TopLeftCardView = new(TopLeftInfoDisplayer);
			TopLeftCardView.FocusChange += (_, change) =>
			{
				change.Old?.ClientCardController.ShowFocused(false);
				change.New.ClientCardController.ShowFocused(true);
			};
		}

		/// <summary>
        /// If applicable, tries to play the current FocusedCard to <paramref name="space"/>
        /// </summary>
		public void Select(Space space)
		{
			//TODO make client notifier a static helper class
			GD.Print($"Selecting {space}");
			FocusedCard?.ClientGame.ClientGameController.Notifier.RequestPlay(FocusedCard, space.x, space.y);
			currentSearch?.Select(space);
		}

		public void Select(ClientGameCard? card)
		{
			GD.Print($"Selecting {card}");
			TopLeftCardView?.Select(card);
			currentSearch?.Select(card);
		}

		public void Highlight(ClientGameCard? card)
		{
			//GD.Print($"Selecting {card}");
			TopLeftCardView?.Hover(card);
		}

		public void StartCardSearch(IEnumerable<int> potentialTargetIDs, IListRestriction listRestriction, string targetBlurb)
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			currentSearch = CardSearch.StartSearch(potentialTargetIDs.Select(GameController.Game.LookupCardByID), listRestriction,
				GameController.Game, this, GameController.Notifier);

			if (currentSearch == null)
			{
				GD.PrintErr("Failed to initalize search.");
				return;
			}

			GameController.CurrentStateController.ShowCurrentStateInfo(targetBlurb);
			currentSearch.SearchFinished += (_, _) => FinishSearch();
		}

		public void StartHandSizeSearch(IEnumerable<int> cardIDs, IListRestriction listRestriction)
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			currentSearch = new HandSizeSearch(cardIDs.Select(GameController.Game.LookupCardByID), listRestriction,
				GameController.Game, this, GameController.Notifier);
			GameController.CurrentStateController.ShowCurrentStateInfo($"Reshuffle down to hand size");
			currentSearch.SearchFinished += (_, _) => FinishSearch();
		}

		public void StartSpaceSearch(IEnumerable<Space> spaces, string blurb)
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			currentSearch = new SpaceSearch(spaces, GameController.Notifier);
			GameController.CurrentStateController.ShowCurrentStateInfo(blurb);
			currentSearch.SearchFinished += (_, _) => FinishSearch();
		}

		private void FinishSearch()
		{
			currentSearch = null;
		}

		public void TargetAccepted() { }

		public void DeclineFurtherTargets()
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			FinishSearch();
			GameController.Notifier.DeclineAnotherTarget();
		}

		public bool IsValidTarget(GameCard card) => currentSearch?.IsValidTarget(card) ?? false;
		public bool IsSelectedTarget(GameCard card) => currentSearch?.IsCurrentTarget(card) ?? false;
		public bool IsUnselectedValidTarget(GameCard card) => IsValidTarget(card) && !IsSelectedTarget(card);
	}
}