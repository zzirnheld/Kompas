using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models
{
	public class ServerTrigger : Trigger
	{
		public ServerTrigger(TriggerData triggerData, Effect effect) : base(triggerData, effect)
		{
			throw new System.NotImplementedException();
		}

		public override GameCard Source => throw new System.NotImplementedException();

		public override Effect Effect => throw new System.NotImplementedException();
	}
}