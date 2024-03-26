using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class AttacksThisTurn : ContextlessLeafIdentityBase<int>
	{
		protected override int AbstractItem
		{
			get
			{
				var card = InitializationContext.source
					?? throw new IllDefinedException();
				return card.AttacksThisTurn;
			}
		}
	}
}