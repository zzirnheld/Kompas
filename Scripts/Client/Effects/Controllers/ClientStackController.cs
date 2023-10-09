using System.Collections.Generic;
using Kompas.Client.Effects.Models;
using Kompas.Client.Effects.Views;
using Kompas.Effects;
using Kompas.Effects.Models;
using Kompas.Gamestate;

namespace Kompas.Client.Effects.Controllers
{
	public class ClientStackController : IStackController
	{
		public ClientStackView clientStackPanelCtrl;

		private readonly EffectStack<IClientStackable, IResolutionContext> stack = new();

		public IEnumerable<IClientStackable> StackEntries => stack.StackEntries;

		public IStackable CurrStackEntry => throw new System.NotImplementedException();

		IEnumerable<IStackable> IStackController.StackEntries => throw new System.NotImplementedException();

		public bool NothingHappening => throw new System.NotImplementedException();

		public void Add(IClientStackable stackable, IResolutionContext context = default)
		{
			stack.Push((stackable, context));
			//clientStackPanelCtrl.Add(stackable);
		}

		public void Remove(int index)
		{
			stack.Cancel(index);
		}
	}
}