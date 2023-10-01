using Godot;

namespace Kompas.UI.DeckBuilder
{
	public partial class BuiltDeckContainer : Control
	{
		public override void _Ready()
		{
			base._Ready();
			Resized += Resize;
		}

		private void Resize()
		{
			foreach (var child in GetChildren())
			{
				if (child is Control ctrl) ResizeChild(ctrl);
			}
		}

		public void AddChild(Control child)
		{
			base.AddChild(child);
			ResizeChild(child);
		}

		private void ResizeChild(Control child)
		{
			child.LayoutMode = 0;
			child.Size = new Vector2(Size.X / 8f, Size.X / 8f);

			float column = child.GetIndex() % 8;
			float row = (float) (child.GetIndex() / 8);
			child.Position = new(child.Size.X * column, child.Size.Y * row); //todo padding?
			GD.Print($"{Size / 8f}, {child.Size}, {child.Size.Y * row} for row {row}, {child.Position}");
		}

		public void MoveChild(Control childNode, int toIndex)
		{
			base.MoveChild(childNode, toIndex);
			Resize();
		}
	}
}