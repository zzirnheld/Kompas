using System.Collections.Generic;
using Godot;

namespace Kompas.Shared.Controllers
{
	/// <summary>
    /// This would be an interface if godot could handle interfaces as [Export] fields
    /// </summary>
	public abstract partial class NodeArranger : Node3D
	{
		public abstract void Arrange(IReadOnlyCollection<Node3D> nodes);

		public abstract void Open();
		public abstract void Close();
	}
}