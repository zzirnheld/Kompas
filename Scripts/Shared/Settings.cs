using Godot;
using Newtonsoft.Json;

namespace Kompas.Shared
{
	public abstract class Settings
	{
		public static readonly Color DefaultFriendlyBlue = new("4a4e9c");
		public static readonly Color DefaultEnemyRed = new("ff3531");

		public static readonly Color FriendlyGold = new("e2a600");
		public static readonly Color EnemySilver = new("808080");
		public string friendlyColorString;
		public string enemyColorString;
		public int friendlyColorIndex = 0;
		public int enemyColorIndex = 0;

		[JsonIgnore]
		public Color FriendlyColor
		{
			set => friendlyColorString = value.ToHtml();
			get => new(friendlyColorString);
		}

		[JsonIgnore]
		public Color EnemyColor
		{
			set => enemyColorString = value.ToHtml();
			get => new(enemyColorString);
		}
	}
}