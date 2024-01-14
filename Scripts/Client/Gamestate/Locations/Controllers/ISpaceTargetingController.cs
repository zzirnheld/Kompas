using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Controllers
{
    public partial interface ISpaceTargetingController
	{
		public Space Space {get;}
		/// <summary>
		/// Toggles the given <paramref name="highlight"/>,
		/// clearing anything that would be mutually exclusive with it.
		/// For example, when you set "can play",
		/// you want to clear out any old indications like "can move" to not be confusing
		/// </summary>
		public void ToggleHighlight(SpaceHighlight highlight, bool show);
	}
}