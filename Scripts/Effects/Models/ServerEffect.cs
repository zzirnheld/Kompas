using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public class ServerEffect : Effect
	{
		public override Game Game => throw new System.NotImplementedException();

		public override Player Controller { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public override Subeffect[] Subeffects => throw new System.NotImplementedException();

		//this is just subeffects, not serverSubeffects, because I originally wanted to be able to load the subeffects client and server side. might still do something like that for an editor
		public ServerSubeffect[] subeffects;

		public override Trigger Trigger => throw new System.NotImplementedException();
	}
}