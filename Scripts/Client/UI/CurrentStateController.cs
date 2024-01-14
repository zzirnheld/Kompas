using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.UI
{
	public partial class CurrentStateController : Control
	{
		[Export]
		private Control? _friendlyTurnParent;
		private Control FriendlyTurnParent => _friendlyTurnParent
			?? throw new UnassignedReferenceException();
		[Export]
		private Control? _enemyTurnParent;
		private Control EnemyTurnParent => _enemyTurnParent
			?? throw new UnassignedReferenceException();

		[Export]
		private Label? _currentStateLabel;
		private Label CurrentStateLabel => _currentStateLabel
			?? throw new UnassignedReferenceException();

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