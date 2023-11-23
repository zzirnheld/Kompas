using Godot;
using Kompas.Client.Cards.Controllers;
using Kompas.Client.Networking;
using Kompas.Effects.Models;
using System.Collections.Generic;

namespace Kompas.Client.UI
{
	public partial class UseEffectDialog : Control
	{
		[Export]
		private Camera3D Camera { get; set; }
		[Export]
		private Label CardName { get; set; }
		[Export]
		private PackedScene EffectButtonPrefab { get; set; }
		[Export]
		private Node EffectButtonsParent { get; set; }
		[Export]
		private Button CancelButton { get; set; }

		private readonly IList<UseEffectDialogButton> buttons = new List<UseEffectDialogButton>();
		private ClientNotifier clientNotifier;

		public override void _Ready()
		{
			CancelButton.Pressed += Unshow;
		}

		public void Display(ClientCardController cardController)
		{
			clientNotifier = cardController.Card.ClientGame.ClientGameController.Notifier;

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
			clientNotifier.RequestActivateEffect(effect.Card, effect.EffectIndex);
		}

		private void Unshow() => Visible = false;
	}
}