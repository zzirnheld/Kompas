using Godot;

namespace Kompas.Godot
{
	public static class NodeExtensions
	{
		public static void TransferChild(this Node parent, Node child)
		{
			child.GetParent()?.RemoveChild(child);
			parent.AddChild(child);
		}
	}
}
