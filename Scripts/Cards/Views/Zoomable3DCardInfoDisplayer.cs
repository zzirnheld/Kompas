using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Views
{
	/// <summary>
    /// currently mostly a placeholder/wrapper for multiple card info displayers. later, should account for stuff like:
    /// Whether the camera is zoomed in or not
    /// Whether the card is a char or not
    /// TODO: should the material for the card image maybe be on this parent?
    /// TODO also the in/out 3d info displayers maybe should inherit from a shared class that handles card frames
    /// (that way each zoom level handles its own frame objects' material setting based on player owner)
    /// </summary>
	public partial class Zoomable3DCardInfoDisplayer : Node3D, ICardInfoDisplayer
	{
		[Export]
		private MeshCardInfoDisplayerBase ZoomedOut { get; set; }

		[Export]
		private MeshCardInfoDisplayerBase ZoomedIn { get; set; }

		[Export]
		private BaseMaterial3D CardImageMaterial { get; set; }

		public bool ShowingInfo { set => Visible = value; }

		public void DisplayCardImage(CardBase card)
		{
			CardImageMaterial.AlbedoTexture = card.CardFaceImage;
		}

		public void DisplayCardNumericStats(CardBase card)
		{
			ZoomedOut.DisplayCardNumericStats(card);
			ZoomedIn.DisplayCardNumericStats(card);
		}

		public void DisplayCardRulesText(CardBase card)
		{
			ZoomedOut.DisplayCardRulesText(card);
			ZoomedIn.DisplayCardRulesText(card);
		}

		public void DisplayFrame(bool friendly)
		{
			ZoomedOut.DisplayFrame(friendly);
			ZoomedIn.DisplayFrame(friendly);
		}

		//FUTURE: replace with enum?
		public void DisplayZoomed(bool zoomedIn)
		{
			ZoomedOut.ShowingInfo = !zoomedIn;
			ZoomedIn.ShowingInfo = zoomedIn;
		}
	}
}