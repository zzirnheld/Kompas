using Godot;

namespace Kompas.UI.DeckBuilder
{
	//IMPL NOTE: I can't override the AddChild methods 1) because they take in nodes, not controls and 2) because they're not virtual
	public partial class BuiltDeckContainer : Control
	{
		private const int PositionalLayoutMode = 0;

		[Export]
		private int ColumnCount { get; set; } = 8;

		[Export]
		private float Padding { get; set; } = 5f;

		public override void _Ready()
		{
			base._Ready();
			Resized += Resize;
		}

		public void AddChild(Control child)
		{
			base.AddChild(child);
			ResizeChild(child);
		}

		public void MoveChild(Control childNode, int toIndex)
		{
			base.MoveChild(childNode, toIndex);
			Resize();
		}

		private void Resize()
		{
			foreach (var child in GetChildren())
			{
				if (child is Control ctrl) ResizeChild(ctrl);
			}
		}

		private void ResizeChild(Control child)
		{
			child.LayoutMode = PositionalLayoutMode;
			//When determining offset, we add padding to the size to account for the padding we're adding after the last element
			float offset = (Size.X + Padding) / ColumnCount;
			float tileSize = offset - Padding;
			child.Size = new Vector2(tileSize, tileSize);

			float column = child.GetIndex() % ColumnCount;
			float row = child.GetIndex() / ColumnCount;
			child.Position = new(offset * column, offset * row); //todo padding?
		}
	}
}