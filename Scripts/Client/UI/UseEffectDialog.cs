using Godot;
using Kompas.Client.Cards.Controllers;
using Kompas.Client.Gamestate;
using Kompas.Client.Networking;
using Kompas.Effects.Models;
using Kompas.Shared.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Kompas.Client.UI;

public partial class UseEffectDialog : Control
{
	[Export]
	private Camera3D? _camera;
	private Camera3D Camera => _camera
		?? throw new UnassignedReferenceException(nameof(_camera), this);

	[Export]
	private Label? _cardName;
	private Label CardName => _cardName
		?? throw new UnassignedReferenceException(nameof(_cardName), this);

	[Export]
	private PackedScene? _effectButtonPrefab;
	private PackedScene EffectButtonPrefab => _effectButtonPrefab
		?? throw new UnassignedReferenceException(nameof(_effectButtonPrefab), this);

	[Export]
	private Node? _effectButtonsParent;
	private Node EffectButtonsParent => _effectButtonsParent
		?? throw new UnassignedReferenceException(nameof(_effectButtonsParent), this);

	[Export]
	private Button? _cancelButton;
	private Button CancelButton => _cancelButton
		?? throw new UnassignedReferenceException(nameof(_cancelButton), this);

	[Export]
	private ClientTargetingController? _targetingController;
	private ClientTargetingController TargetingController => _targetingController
		?? throw new UnassignedReferenceException(nameof(_targetingController), this);

	[Export]
	private AttackDialogButton? _attackDialogButton;
	private AttackDialogButton AttackDialogButton => _attackDialogButton
		?? throw new UnassignedReferenceException(nameof(_attackDialogButton), this);

	private readonly IList<UseEffectDialogButton> buttons = new List<UseEffectDialogButton>();
	private ClientNotifier? _clientNotifier;
	private ClientNotifier ClientNotifier => _clientNotifier
		?? throw new NotInitializedException();

	public override void _Ready()
	{
		CancelButton.Pressed += Unshow;
	}

	public void Display(ClientCardController cardController)
	{
		_clientNotifier = cardController.Card.ClientGame.ClientGameController.Notifier;

		var localPlayer = cardController.Card.ClientGame.FriendlyPlayer;
		var effects = cardController.Card.Effects
			.Where(eff => eff.ActivationRestriction != null)
			//TODO config/debug mode for whether can currently activate normally
			.Where(eff => eff.ActivationRestriction?.IsValid(localPlayer, IResolutionContext.PlayerAction(localPlayer)) ?? false);
		CardName.Text = cardController.Card.CardName;
		
		foreach (var child in buttons)
		{
			EffectButtonsParent.RemoveChild(child);
			child.QueueFree();
		}
		buttons.Clear();

		foreach (var effect in effects)
		{
			var effectButton = EffectButtonPrefab.Instantiate<UseEffectDialogButton>();
			effectButton.Init(this, effect);
			EffectButtonsParent.AddChild(effectButton);
			buttons.Add(effectButton);
		}

		AttackDialogButton.IsNowFor(cardController);
		var selectedCard = TargetingController.SelectedCard;
		AttackDialogButton.Visible = selectedCard is not null
			&& selectedCard.AttackingDefenderRestriction.IsValid(cardController.Card, IResolutionContext.PlayerAction(selectedCard.ControllingPlayer));

		Position = Camera.UnprojectPosition(cardController.GlobalPosition);
		Visible = true;
		//move to end of list
		AttackDialogButton.MoveToFront();
		CancelButton.MoveToFront();
	}

	public void Activate(Effect effect)
	{
		Visible = false;
		if (effect.Card == null) return;
		ClientNotifier.RequestActivateEffect(effect.Card, effect.EffectIndex);
	}

	public void Attack(ClientCardController cardController)
	{
		if (TargetingController.SelectedCard is null)
		{
			throw new System.InvalidOperationException("Can't have a null selected card attack the currently right-clicked card!");
		}
		Visible = false;
		ClientNotifier.RequestAttack(TargetingController.SelectedCard, cardController.Card);
	}

	private void Unshow() => Visible = false;
}