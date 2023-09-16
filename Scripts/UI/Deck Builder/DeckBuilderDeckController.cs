using Godot;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderDeckController : Node
	{
		[Export]
		private Control DeckNodesParent { get; set; }

		[Export]
		private PackedScene DeckBuilderInfoDisplayerPrefab { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if (DeckBuilderInfoDisplayerPrefab.Instantiate() is not DeckBuilderBuiltDeckInfoDisplayer card)
				throw new System.ArgumentNullException(nameof(DeckBuilderInfoDisplayerPrefab), "Was not the right type");
			DeckNodesParent.AddChild(card);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}