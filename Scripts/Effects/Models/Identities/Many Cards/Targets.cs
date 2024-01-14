using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Targets : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		protected override IReadOnlyCollection<IGameCardInfo> AbstractItem
		{
			get
			{
				var effect = InitializationContext.effect
					?? throw new IllDefinedException();
				return effect.CardTargets.ToArray();
			}
		}
	}
}