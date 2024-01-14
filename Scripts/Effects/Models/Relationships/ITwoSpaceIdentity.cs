using Kompas.Gamestate;

namespace Kompas.Effects.Models.Relationships.Spaces
{
	/// <summary>
	/// A description of one space, based on two others
	/// </summary>
	public interface ITwoSpaceIdentity
	{
		public Space? SpaceFrom(Space first, Space second);
	}

	namespace TwoSpaceIdentities
	{
		public class Displacement : ITwoSpaceIdentity
		{
			public Space? SpaceFrom(Space first, Space second) => first.DisplacementTo(second);
		}

		public class SpaceBetween : ITwoSpaceIdentity
		{
			public Space? SpaceFrom(Space first, Space second) => first.DirectlyBetween(second);
		}
	}
}