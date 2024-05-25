

using System.Collections.Generic;

namespace Kompas.Effects.Models
{
	public class TriggerCapturer
	{

		private readonly IReadOnlyCollection<FullTriggerContext> contexts;

		public TriggerCapturer(IReadOnlyCollection<FullTriggerContext> contexts)
		{
			this.contexts = contexts;
		}

		public delegate void CapturedEvent();

		public void Capture(CapturedEvent capturedEvent)
		{
			capturedEvent();
			foreach (var context in contexts)
			{
				context.Context.CacheAfterEvent();
			}
		}
	}
}