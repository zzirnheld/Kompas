using System.Collections.Generic;
using Godot;
using Kompas.Shared.Enumerable;

namespace Kompas.Shared.Controllers
{
	public partial class PlaceAtEachController : Node
	{
		[Export]
		private Node[] Nodes { get; set; }
		[Export]
		private Node Overflow { get; set; }

		public void Spread(IReadOnlyCollection<Node> nodes)
		{
			foreach (var (index, node) in nodes.Enumerate())
			{
				node.GetParent()?.RemoveChild(node);
				Nodes[index].AddChild(node);
			}
		}
	}
}