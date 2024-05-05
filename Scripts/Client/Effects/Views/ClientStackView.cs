using System.Collections.Generic;
using Godot;
using Kompas.Client.Effects.Models;
using Kompas.Client.UI;
using Kompas.Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Effects.Views
{
	public partial class ClientStackView : Node 
	{
		[Export]
		private CurrentStateController? _currentStateController;
		private CurrentStateController CurrentStateController => _currentStateController
			?? throw new UnassignedReferenceException(nameof(_currentStateController));

		[Export]
		private Control? _stackElementsParent;
		private Control StackElementsParent => _stackElementsParent
			?? throw new UnassignedReferenceException(nameof(_stackElementsParent));

		[Export]
		private Control? _currentlyResolvingParent;
		private Control CurrentlyResolvingParent => _currentlyResolvingParent
			?? throw new UnassignedReferenceException(nameof(_currentlyResolvingParent));

		[Export]
		private PackedScene? _effectStackableView;
		private PackedScene EffectStackableView => _effectStackableView
			?? throw new UnassignedReferenceException(nameof(_effectStackableView));

		[Export]
		private PackedScene? _attackStackableView;
		private PackedScene AttackStackableView => _attackStackableView
			?? throw new UnassignedReferenceException(nameof(_attackStackableView));

		private readonly Dictionary<IClientStackable, ClientStackElementView> stackableToView = new();

        public void Activated(ClientEffect effect)
		{
			//TODO initialize the stackable view by the effect. should be a function on the EffectStackableView
		}

		public void Attacked(ClientAttack attack)
		{

		}

		public void Resolving(IClientStackable stackable)
		{
			var view = stackableToView[stackable];
			stackableToView.Remove(stackable);
			CurrentlyResolvingParent.QueueFreeChildren();
			CurrentlyResolvingParent.TransferChild(view);
		}

		public void Cancel(IClientStackable stackable)
		{
			stackableToView[stackable].QueueFree();
			stackableToView.Remove(stackable);
		}

		public void StackEmptied(IClientStackable stackable)
		{
			CurrentlyResolvingParent.QueueFreeChildren();
		}
	}
}