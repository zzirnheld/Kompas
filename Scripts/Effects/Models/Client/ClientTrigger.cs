
using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Effects.Models.Client
{
	public class ClientTrigger : Trigger
	{
		private readonly ClientEffect effect;

		public override GameCard Source => effect.Source;
		public override Effect Effect => effect;

		public ClientTrigger(TriggerData triggerData, ClientEffect effect) : base(triggerData, effect)
		{
			this.effect = effect;
		}
	}
}