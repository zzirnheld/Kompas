using Godot;
using Kompas.Effects.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.UI
{
	public partial class UseEffectDialogButton : Node
	{
		[Export]
		private Label? _effectName;
		private Label EffectName => _effectName
			?? throw new UnassignedReferenceException();
		[Export]
		private Button? _button;
		private Button Button => _button
			?? throw new UnassignedReferenceException();

		private UseEffectDialog? _dialog;
		private UseEffectDialog Dialog => _dialog
			?? throw new UnassignedReferenceException();
		private Effect? _effect;
		private Effect Effect => _effect
			?? throw new UnassignedReferenceException();

		public override void _Ready()
		{
			Button.Pressed += ActivateEffect;
		}

		public void Init(UseEffectDialog dialog, Effect effect)
		{
			_dialog = dialog;
			_effect = effect;
			EffectName.Text = effect.blurb;
		}

		private void ActivateEffect() => Dialog.Activate(Effect);
	}
}