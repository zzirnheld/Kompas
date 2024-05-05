using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Kompas.Client.Effects.Models;
using Kompas.Client.UI;
using Kompas.Effects.Models;
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

		//TODO: how to differentiate different activation
		private readonly Dictionary<IResolvingStackable, ClientStackableView> stackableToView = new();


        public void Activated(IResolvingStackable<ClientEffect> stackable)
		{
			var view = EffectStackableView.Instantiate<ClientEffectStackableView>();
			view.Initialize(stackable.Stackable);
			stackableToView[stackable] = view;

			StackElementsParent.AddChild(view);
		}

		public void Attacked(IResolvingStackable<ClientAttack> stackable)
		{	
			var view = EffectStackableView.Instantiate<ClientAttackStackableView>();
			view.Initialize(stackable.Stackable);
			stackableToView[stackable] = view;

			StackElementsParent.AddChild(view);
		}

		public void Resolving(IResolvingStackable? stackable)
		{
			if (stackable == null)
			{
				StackEmptied();
				return;
			}
			var view = stackableToView[stackable];
			stackableToView.Remove(stackable);
			CurrentlyResolvingParent.QueueFreeChildren();
			CurrentlyResolvingParent.TransferChild(view);
		}

		public void Cancel(IResolvingStackable stackable)
		{
			stackableToView[stackable].QueueFree();
			stackableToView.Remove(stackable);
		}

		public void StackEmptied()
		{
			StackElementsParent.QueueFreeChildren();
			CurrentlyResolvingParent.QueueFreeChildren();
		}
	}
}