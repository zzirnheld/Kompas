using Godot;
using Kompas.Client.Cards.Controllers;
using Kompas.Client.Networking;
using Kompas.Effects.Models;
using Kompas.Shared.Exceptions;
using System.Collections.Generic;

namespace Kompas.Client.UI
{
	public partial class UseEffectDialog : Control
	{
		[Export]
		private Camera3D? _camera;
		private Camera3D Camera => _camera
			?? throw new UnassignedReferenceException();
		[Export]
		private Label? _cardName;
		private Label CardName => _cardName
			?? throw new UnassignedReferenceException();
		[Export]
		private PackedScene? _effectButtonPrefab;
		private PackedScene EffectButtonPrefab => _effectButtonPrefab
			?? throw new UnassignedReferenceException();
		[Export]
		private Node? _effectButtonsParent;
		private Node EffectButtonsParent => _effectButtonsParent
			?? throw new UnassignedReferenceException();
		[Export]
		private Button? _cancelButton;
		private Button CancelButton => _cancelButton
			?? throw new UnassignedReferenceException();

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

			var effects = cardController.Card.Effects;
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

			Position = Camera.UnprojectPosition(cardController.GlobalPosition);
			Visible = true;
			CancelButton.MoveToFront(); //moves to end of list
		}

		public void Activate(Effect effect)
		{
			Visible = false;
			if (effect.Card == null) return;
			ClientNotifier.RequestActivateEffect(effect.Card, effect.EffectIndex);
		}

		private void Unshow() => Visible = false;
	}
}