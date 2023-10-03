using Kompas.Shared;

namespace Kompas.Client
{
	public enum StatHighlight { NoHighlight, ColoredBack }
	public enum ConfirmTargets { No, Prompt }

	public class ClientSettings : Settings
	{

		public const float DefaultZoomThreshold = 14f;

		public StatHighlight statHighlight;
		public float zoomThreshold;
		public ConfirmTargets confirmTargets;
		public bool showAdvancedEffectsSettings = false;
		public string defaultIP;

		public static ClientSettings Default => new()
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
		public ClientSettings Cleanup()
		{
			if (zoomThreshold == default) zoomThreshold = DefaultZoomThreshold;

			return this;
		}
	}
}