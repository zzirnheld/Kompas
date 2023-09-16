using Godot;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderDeckController : Node
	{
		[Export]
		private Control DeckNodesParent { get; set; }

		[Export]
		private PackedScene DeckBuilderInfoDisplayerPrefab { get; set; }

		public override void _Ready()
		{
			foreach (var node in DeckNodesParent.GetChildren())
			{
				node.QueueFree();
			}
			for (int i = 0; i < 48; i++)
			{
				if (DeckBuilderInfoDisplayerPrefab.Instantiate() is not DeckBuilderBuiltDeckInfoDisplayer card)
					throw new System.ArgumentNullException(nameof(DeckBuilderInfoDisplayerPrefab), "Was not the right type");
				DeckNodesParent.AddChild(card);
			}
		}
	}
}