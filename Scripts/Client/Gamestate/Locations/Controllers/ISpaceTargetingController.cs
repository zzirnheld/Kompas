using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Controllers
{
	public interface ISpaceTargetingController
	{
		public Space Space {get;}
		public void ShowCanMoveHere(bool can);
	}
}