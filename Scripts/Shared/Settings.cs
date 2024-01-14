using Godot;
using Newtonsoft.Json;

namespace Kompas.Shared
{
	public class Settings
	{
		public enum StatHighlight { NoHighlight, ColoredBack }
		public enum ConfirmTargets { No, Prompt }

		public const float DefaultZoomThreshold = 14f;

		public static readonly Color DefaultFriendlyBlue = new("4a4e9c");
		public static readonly Color DefaultEnemyRed = new("ff3531");

		public static readonly Color FriendlyGold = new("e2a600");
		public static readonly Color EnemySilver = new("808080");

		public string? friendlyColorString;
		public string? enemyColorString;
		public int friendlyColorIndex = 0;
		public int enemyColorIndex = 0;

		public StatHighlight? statHighlight;
		public float zoomThreshold;
		public ConfirmTargets? confirmTargets;
		public bool showAdvancedEffectsSettings = false;
		public string? defaultIP;

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
		
		public static Settings Default => new()
		{
			statHighlight = StatHighlight.NoHighlight,
			zoomThreshold = DefaultZoomThreshold,
			confirmTargets = ConfirmTargets.No,
			showAdvancedEffectsSettings = false,
			defaultIP = "",
			FriendlyColor = DefaultFriendlyBlue,
			EnemyColor = DefaultEnemyRed,
			friendlyColorIndex = 0,
			enemyColorIndex = 0
		};

		/// <summary>
		/// Updates any json-default values to their regular defaults
		/// </summary>
		/// <returns><see cref="this"/></returns>
		public Settings Cleanup()
		{
			if (zoomThreshold == default) zoomThreshold = DefaultZoomThreshold;

			return this;
		}
	}
}