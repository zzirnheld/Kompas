using Godot;

namespace Kompas.Godot
{
	public static class NodeExtensions
	{
		/// <summary>
        /// Removes the child from its previous parent, and adds it as a child of this node.
        /// </summary>
		public static void TransferChild(this Node parent, Node child)
		{
			child.GetParent()?.RemoveChild(child);
			parent.AddChild(child);
		}
	}
}
