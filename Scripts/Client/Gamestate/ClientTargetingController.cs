using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.Gamestate.Search;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Gamestate
{
	public partial class ClientTargetingController : Node
	{
		[Export]
		private ControlInfoDisplayer? _topLeftInfoDisplayer;
		private ControlInfoDisplayer TopLeftInfoDisplayer => _topLeftInfoDisplayer ?? throw new UnassignedReferenceException();
		[Export]
		private ClientGameController? _gameController;
		private ClientGameController GameController => _gameController ?? throw new UnassignedReferenceException();
		[Export]
		private Control? _canDeclineFurtherTargetsButton;
		private Control CanDeclineFurtherTargetsButton => _canDeclineFurtherTargetsButton ?? throw new UnassignedReferenceException();

		private ClientTopLeftCardView? _topLeftCardView;
		public ClientTopLeftCardView TopLeftCardView => _topLeftCardView ?? throw new NotReadyYetException();

		/// <summary>
		/// The view already contains the logic for focusing on a given card, for entirely historical reasons.
		/// I could rip that out and move it here, but why bother
		/// </summary>
		public ClientGameCard? FocusedCard => TopLeftCardView.FocusedCard;
		public ClientGameCard? ShownCard => TopLeftCardView.ShownCard;

		private ISearch? _currentSearch;
		private ISearch? CurrentSearch
		{
			get => _currentSearch;
			set
			{
				_currentSearch = value;
				foreach (var card in GameController.Game.Cards) card.CardController.RefreshTargeting();
			}
		}

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
			_topLeftCardView = new(TopLeftInfoDisplayer);
			TopLeftCardView.FocusChange += (_, change) =>
			{
				change.Old?.ClientCardController.ShowFocused(false);
				change.New?.ClientCardController.ShowFocused(true);
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
			CurrentSearch?.Select(space);
		}

		public void Select(ClientGameCard? card)
		{
			GD.Print($"Selecting {card}");
			TopLeftCardView?.Select(card);
			if (card == null) return;

			CurrentSearch?.Select(card);
		}

		public void Highlight(ClientGameCard? card)
		{
			//GD.Print($"Selecting {card}");
			TopLeftCardView?.Hover(card);
		}

		public void StartCardSearch(IEnumerable<int> potentialTargetIDs, IListRestriction listRestriction, string targetBlurb)
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			CurrentSearch = CardSearch.StartSearch(potentialTargetIDs.Select(GameController.Game.LookupCardByID).NonNull(), listRestriction,
				GameController.Game, this, GameController.Notifier);

			if (CurrentSearch == null)
			{
				GD.PrintErr("Failed to initalize search.");
				return;
			}

			GameController.CurrentStateController.ShowCurrentStateInfo(targetBlurb);
			CurrentSearch.SearchFinished += (_, _) => FinishSearch();
		}

		public void StartHandSizeSearch(IEnumerable<int> cardIDs, IListRestriction listRestriction)
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			CurrentSearch = new HandSizeSearch(cardIDs.Select(GameController.Game.LookupCardByID).NonNull(), listRestriction,
				GameController.Game, this, GameController.Notifier);
			GameController.CurrentStateController.ShowCurrentStateInfo($"Reshuffle down to hand size");
			CurrentSearch.SearchFinished += (_, _) => FinishSearch();
		}

		public void StartSpaceSearch(IEnumerable<Space> spaces, string blurb)
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			CurrentSearch = new SpaceSearch(spaces, GameController.Notifier);
			GameController.CurrentStateController.ShowCurrentStateInfo(blurb);
			CurrentSearch.SearchFinished += (_, _) => FinishSearch();
		}

		private void FinishSearch()
		{
			CurrentSearch = null;
		}

		public void TargetAccepted() { }

		public void DeclineFurtherTargets()
		{
			_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
			FinishSearch();
			GameController.Notifier.DeclineAnotherTarget();
		}

		public bool IsValidTarget(GameCard card) => CurrentSearch?.IsValidTarget(card) ?? false;
		public bool IsSelectedTarget(GameCard card) => CurrentSearch?.IsCurrentTarget(card) ?? false;
		public bool IsUnselectedValidTarget(GameCard card) => IsValidTarget(card) && !IsSelectedTarget(card);
	}
}