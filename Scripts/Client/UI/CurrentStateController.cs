using Godot;

namespace Kompas.Client.UI
{
	public partial class CurrentStateController : Control
	{
		[Export]
		private Control FriendlyTurnParent { get; set; }
		[Export]
		private Control EnemyTurnParent { get; set; }

		public void ChangeTurn(bool friendly)
		{
			FriendlyTurnParent.Visible = friendly;
			EnemyTurnParent.Visible = !friendly;
		}
	}
}