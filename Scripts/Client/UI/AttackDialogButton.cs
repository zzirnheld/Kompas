using Godot;
using Kompas.Client.Cards.Controllers;
using Kompas.Effects.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.UI;

public partial class AttackDialogButton : Button
{
	[Export]
	private Button? _button;
	private Button Button => _button
		?? throw new UnassignedReferenceException();

	[Export]
	private UseEffectDialog? _dialog;
	private UseEffectDialog Dialog => _dialog
		?? throw new UnassignedReferenceException();

	private ClientCardController? _card;
	private ClientCardController Card => _card
		?? throw new NotInitializedException();

	public override void _Ready()
	{
		Button.Pressed += Attack;
	}

	public void IsNowFor(ClientCardController cardController)
	{
		_card = cardController;
	}

	private void Attack() => Dialog.Attack(Card);
}