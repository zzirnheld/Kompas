using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class SpacesController : Node3D
	{
		[Export]
		private PackedScene? _linkedSpaces;
		private PackedScene LinkedSpaces => _linkedSpaces ?? throw new UnassignedReferenceException();

		[Export]
		private Material? _defaultAOEMaterial;
		private Material DefaultAOEMaterial => _defaultAOEMaterial ?? throw new UnassignedReferenceException();

		[Export]
		private ClientGameController? _gameController;
		private ClientGameController GameController => _gameController ?? throw new UnassignedReferenceException();
		//if this becomes a shared controller, this will need to be moved to a child class

		[Export]
		private LinkedSpacesController? _canMove;
		private LinkedSpacesController CanMove => _canMove ?? throw new UnassignedReferenceException();
		[Export]
		private Material? _canMoveMaterial;
		private Material CanMoveMaterial => _canMoveMaterial ?? throw new UnassignedReferenceException();

		[Export]
		private LinkedSpacesController? _canPlay;
		private LinkedSpacesController CanPlay => _canPlay ?? throw new UnassignedReferenceException();
		[Export]
		private Material? _canPlayMaterial;
		private Material CanPlayMaterial => _canPlayMaterial ?? throw new UnassignedReferenceException();

		[Export]
		private LinkedSpacesController? _recommend;
		private LinkedSpacesController Recommend => _recommend ?? throw new UnassignedReferenceException();
		[Export]
		private LinkedSpacesController? _unrecommend;
		private LinkedSpacesController Unrecommend => _unrecommend
			?? throw new UnassignedReferenceException(nameof(_unrecommend), this);
		[Export]
		private Material? _recommendMaterial;
		private Material RecommendMaterial => _recommendMaterial
			?? throw new UnassignedReferenceException(nameof(_recommendMaterial), this);
		[Export]
		private Material? _unrecommendMaterial;
		private Material UnrecommendMaterial => _unrecommendMaterial
			?? throw new UnassignedReferenceException(nameof(_unrecommendMaterial), this);

		[Export]
		private SpacesClickingController? _spacesClickingController;
		private SpacesClickingController SpacesClickingController => _spacesClickingController ?? throw new UnassignedReferenceException();

		[Export]
		private PlaceInSpaceController? _placeInSpaceController;
		private PlaceInSpaceController PlaceInSpaceController => _placeInSpaceController ?? throw new UnassignedReferenceException();

		/** Used to adjust height to avoid z-fighting */
		private Node3D? _lastSpacesController;
		private Node3D LastSpacesController
		{
			get => _lastSpacesController ?? throw new NotReadyYetException();
			set => _lastSpacesController = value ?? throw new NullReferenceException();
		}

		public override void _Ready()
		{
			base._Ready();

			CanMove.UpdateMaterial(CanMoveMaterial);
			CanPlay.UpdateMaterial(CanPlayMaterial);
			Recommend.UpdateMaterial(RecommendMaterial);
			Unrecommend.UpdateMaterial(UnrecommendMaterial);

			DisplayNone();

			CanMove.Display(s => s.DistanceTo((2, 3)) <= 2, true);

			LastSpacesController = CanMove;

			SpacesClickingController.LeftClick += (_, tuple) => Clicked(tuple.space, tuple.doubleClick);
		}

		private void Clicked(Space space, bool doubleClick) => GameController.TargetingController.Select(space, doubleClick);

		public void DisplayNone()
		{
			CanMove.Display(_ => false, false);
			CanPlay.Display(_ => false, false);
			Recommend.Display(_ => false, false);
			Unrecommend.Display(_ => false, false);
		}

		public void DisplayCanMove(LinkedSpacesController.ShouldShowSpace predicate)
		{
			CanMove.Display(predicate, false);
			CanPlay.Display(_ => false, false);
			Recommend.Display(_ => false, false);
			Unrecommend.Display(_ => false, false);
		}

		public void DisplayCanPlay(LinkedSpacesController.ShouldShowSpace predicate)
		{
			CanMove.Display(_ => false, false);
			CanPlay.Display(predicate, false);
			Recommend.Display(_ => false, false);
			Unrecommend.Display(_ => false, false);
		}

		public void DisplayCanTarget(LinkedSpacesController.ShouldShowSpace recommend, LinkedSpacesController.ShouldShowSpace unrecommend)
		{
			CanMove.Display(_ => false, false);
			CanPlay.Display(_ => false, false);
			Recommend.Display(recommend, false);
			Unrecommend.Display(unrecommend, false);
		}

		//TODO: this will make the other controller responsible for updating Display on LinkedSpaceController,
		//and destroying it if necessary.
		//Is this what I want?
		public LinkedSpacesController AddAOE()
		{
			if (LinkedSpaces.Instantiate() is not LinkedSpacesController ctrl)
				throw new System.ArgumentNullException(nameof(LinkedSpacesController), "Was not the right type");

			ctrl.GetParent()?.RemoveChild(ctrl);
			AddChild(ctrl);
			ctrl.Position = LastSpacesController.Position + (Vector3.Up * 0.0001f);
			ctrl.Rotation = LastSpacesController.Rotation;
			ctrl.UpdateMaterial(DefaultAOEMaterial);
			LastSpacesController = ctrl;
			return ctrl;
		}

		public void Place(ICardController card) => PlaceInSpaceController.Place(card);
	}
}