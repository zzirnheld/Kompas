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
		private Node3D? _canPlay;
		private Node3D CanPlay => _canPlay ?? throw new UnassignedReferenceException();

		public Space Space => SpaceController.Space;

		private Dictionary<SpaceHighlight, IReadOnlyCollection<SpaceHighlight>> MutualExclusion = new()
		{
			{ SpaceHighlight.CanMove, new SpaceHighlight[] { SpaceHighlight.CanPlay } },
			{ SpaceHighlight.CanPlay, new SpaceHighlight[] { SpaceHighlight.CanMove } },
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
				{ SpaceHighlight.CanPlay, can => CanPlay.Visible = can },
			};
		}
		
		public void ToggleHighlight(SpaceHighlight highlight, bool show)
		{
			HighlightToToggle[highlight](show);
			
			ClearMutuallyExclusiveHighlights(highlight);
		}

		public void ClearMutuallyExclusiveHighlights(SpaceHighlight highlight)
		{
			foreach (var unhighlight in MutualExclusion[highlight])
				HighlightToToggle[unhighlight](false);
		}
	}
}