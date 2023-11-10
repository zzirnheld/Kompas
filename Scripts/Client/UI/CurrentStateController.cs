using Godot;

namespace Kompas.Client.UI
{
	public partial class CurrentStateController : Control
	{
		[Export]
		private Control FriendlyTurnParent { get; set; }
		[Export]
		private Control EnemyTurnParent { get; set; }

		[Export]
		private Label CurrentStateLabel { get; set; }

		public override void _Ready()
		{
			base._Ready();
			CurrentStateLabel.Text = string.Empty;
		}

		public void ChangeTurn(bool friendly)
		{
			FriendlyTurnParent.Visible = friendly;
			EnemyTurnParent.Visible = !friendly;
		}

		public void ShowCurrentStateInfo(string information)
		{
			CurrentStateLabel.Text = information; //Later, a bit more pizzaz and multiple tiers of information
		}
	}
}