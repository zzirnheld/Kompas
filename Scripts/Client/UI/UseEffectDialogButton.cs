using Godot;
using Kompas.Effects.Models;

namespace Kompas.Client.UI
{
	public partial class UseEffectDialogButton : Node
	{
		[Export]
		private Label EffectName { get; set; }
		[Export]
		private Button Button { get; set; }

		private UseEffectDialog dialog;
		private Effect effect;

		public override void _Ready()
		{
			Button.Pressed += ActivateEffect;
		}

		public void Init(UseEffectDialog dialog, Effect effect)
		{
			this.dialog = dialog;
			this.effect = effect;
			EffectName.Text = effect.blurb;
		}

		private void ActivateEffect() => dialog.Activate(effect);
	}
}