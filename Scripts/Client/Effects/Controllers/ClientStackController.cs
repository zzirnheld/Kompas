using System.Collections.Generic;
using Kompas.Client.Effects.Models;
using Kompas.Client.Effects.Views;
using Kompas.Effects;
using Kompas.Effects.Models;

namespace Kompas.Client.Effects.Controllers
{
	public class ClientStackController
	{
		public ClientStackView clientStackPanelCtrl;

		private readonly EffectStack<IClientStackable, IResolutionContext> stack = new();

		public IEnumerable<IClientStackable> StackEntries => stack.StackEntries;

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