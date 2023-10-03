
using Godot;
using Kompas.Client.Gamestate.Players;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Client.Effects.Models
{
	public class DummySubeffect : Subeffect
	{
		public override Effect Effect => ClientEffect;
		public override IPlayer Controller => Effect.ControllingPlayer;
		public override IGame Game => ClientEffect.Game;

		public ClientEffect ClientEffect { get; private set; }

		public static DummySubeffect FromJson(string json, ClientEffect parent, int subeffIndex)
		{
			var subeff = JsonConvert.DeserializeObject<Subeffect>(json);

			GD.Print($"Creating subeffect from json {json}");
			DummySubeffect toReturn;

			toReturn = new DummySubeffect();

			if (toReturn != null)
			{
				GD.Print($"Finishing setup for new effect of type {subeff.GetType()}");
				toReturn.Initialize(parent, subeffIndex);
			}

			return toReturn;
		}

		public virtual void Initialize(ClientEffect eff, int subeffIndex)
		{
			this.ClientEffect = eff;
			this.SubeffIndex = subeffIndex;
		}
	}
}