using System.Collections.Generic;
using Godot;
using Kompas.Client.Effects.Models;
using Kompas.Client.UI;
using Kompas.Effects.Models;
using Kompas.Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Effects.Views;

public partial class ClientStackView : Control 
{
	[Export]
	private CurrentStateController? _currentStateController;
	private CurrentStateController CurrentStateController => _currentStateController
		?? throw new UnassignedReferenceException(nameof(_currentStateController));

	[Export]
	private Control? _stackElementsParent;
	private Control StackElementsParent => _stackElementsParent
		?? throw new UnassignedReferenceException(nameof(_stackElementsParent));

	/// <summary>
	/// The parent node for the currently resolving stackable's view
	/// </summary>
	[Export]
	private Control? _currentlyResolvingParent;
	private Control CurrentlyResolvingParent => _currentlyResolvingParent
		?? throw new UnassignedReferenceException(nameof(_currentlyResolvingParent));

	/// <summary>
	/// The parent node for all of the currently resolving view, including the background.
	/// </summary>
	[Export]
	private Control? _currentlyResolvingPanel;
	private Control CurrentlyResolvingPanel => _currentlyResolvingPanel
		?? throw new UnassignedReferenceException(nameof(_currentlyResolvingPanel));


	[Export]
	private PackedScene? _effectStackableView;
	private PackedScene EffectStackableView => _effectStackableView
		?? throw new UnassignedReferenceException(nameof(_effectStackableView));

	[Export]
	private PackedScene? _attackStackableView;
	private PackedScene AttackStackableView => _attackStackableView
		?? throw new UnassignedReferenceException(nameof(_attackStackableView));

	[Export]
	private PackedScene? _handSizeStackableView;
	private PackedScene HandSizeStackableView => _handSizeStackableView
		?? throw new UnassignedReferenceException(nameof(_handSizeStackableView), this);

	private readonly Dictionary<IResolvingStackable, ClientStackableView> stackableToView = new(new ResolvingStackableEqualityComparer());

	public void Activated(IResolvingStackable<ClientEffect> stackable)
	{
		this.Visible = true;

		var view = EffectStackableView.Instantiate<ClientEffectStackableView>();
		view.Initialize(stackable.Stackable);
		view.MouseEntered += () => stackable.Stackable.Card.CardController.ShowEffectSource(true);
		view.MouseExited += () => stackable.Stackable.Card.CardController.ShowEffectSource(false);

		stackableToView[stackable] = view;

		StackElementsParent.AddChild(view);
	}

	public void Attacked(IResolvingStackable<ClientAttack> stackable)
	{
		this.Visible = true;

		var view = AttackStackableView.Instantiate<ClientAttackStackableView>();
		view.Initialize(stackable.Stackable);
		stackableToView[stackable] = view;

		StackElementsParent.AddChild(view);
	}

	public void HandSize(IResolvingStackable<ClientHandSizeStackable> stackable)
	{
		this.Visible = true;

		var view = HandSizeStackableView.Instantiate<ClientHandSizeStackableView>();
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
		this.Visible = true;
		CurrentlyResolvingPanel.Visible = true;
		//Curiously, when you use an IEqualityComparer, the debugger thinks the key isn't present
		//when it actually is according to the item accessor.
		//Unclear whether this is a language problem or a problem with the VSCode debugger.
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
		CurrentlyResolvingPanel.Visible = false;
		this.Visible = false;
	}
}