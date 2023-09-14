using Godot;
using Newtonsoft.Json;

namespace Kompas.Shared
{
	public abstract class Settings
	{
		public static readonly Color DefaultFriendlyBlue = new(74, 78, 156, 255);
		public static readonly Color DefaultEnemyRed = new(255, 53, 49, 255);

		public static readonly Color FriendlyGold = new(226, 166, 0, 255);
		public static readonly Color EnemySilver = new(128, 128, 128, 255);
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