
using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Client.Effects.Models
{
	public class ClientTrigger : Trigger
	{
		private readonly ClientEffect effect;

		public override GameCard Card => effect.Card;
		public override Effect Effect => effect;

		public ClientTrigger(TriggerData triggerData, ClientEffect effect) : base(triggerData, effect)
		{
			this.effect = effect;
		}
	}
}