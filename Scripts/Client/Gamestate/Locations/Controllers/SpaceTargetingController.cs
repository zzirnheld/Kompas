using System;
using System.Collections.Generic;
using Godot;
using Kompas.Effects.Models.Restrictions.Cards;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;
using static Kompas.Client.Gamestate.Controllers.ISpaceTargetingController;

namespace Kompas.Client.Gamestate.Controllers
{
	public partial class SpaceTargetingController : Node, ISpaceTargetingController
	{
		[Export]
		private SpaceController? _spaceController;
		private SpaceController SpaceController => _spaceController ?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _canMove;
		private Node3D CanMove => _canMove ?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _recommendPlay;
		private Node3D RecommendPlay => _recommendPlay ?? throw new UnassignedReferenceException();

		[Export]
		private Node3D? _unrecommendPlay;
		private Node3D UnrecommendPlay => _unrecommendPlay ?? throw new UnassignedReferenceException();

		public Space Space => SpaceController.Space;

		private readonly Dictionary<SpaceHighlight, IReadOnlyCollection<SpaceHighlight>> mutualExclusion = new()
		{
			{ SpaceHighlight.CanMove, new SpaceHighlight[] { SpaceHighlight.UnrecommendedPlay, SpaceHighlight.RecommendPlay } },
			{ SpaceHighlight.RecommendPlay, new SpaceHighlight[] { SpaceHighlight.UnrecommendedPlay, SpaceHighlight.CanMove } },
			{ SpaceHighlight.UnrecommendedPlay, new SpaceHighlight[] { SpaceHighlight.CanMove, SpaceHighlight.RecommendPlay } },
		};
		private Dictionary<SpaceHighlight, Action<bool>>? _highlightToToggle;
		private Dictionary<SpaceHighlight, Action<bool>> HighlightToToggle => _highlightToToggle
			?? throw new NotReadyYetException();

		public override void _Ready()
		{
			base._Ready();
			_highlightToToggle = new()
			{
				{ SpaceHighlight.CanMove, can => CanMove.Visible = can },
				{ SpaceHighlight.RecommendPlay, can => RecommendPlay.Visible = can },
				{ SpaceHighlight.UnrecommendedPlay, can => UnrecommendPlay.Visible = can },
			};
		}
		
		public void ToggleHighlight(SpaceHighlight highlight, bool show)
		{
			HighlightToToggle[highlight](show);
			
			ClearMutuallyExclusiveHighlights(highlight);
		}

		public void ClearMutuallyExclusiveHighlights(SpaceHighlight highlight)
		{
			foreach (var unhighlight in mutualExclusion[highlight])
				HighlightToToggle[unhighlight](false);
		}
	}
}