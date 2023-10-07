using System.Collections.Generic;
using Kompas.Effects.Models.Client;
using Kompas.Effects.Models;
using Kompas.Effects.Views.Client;

namespace Kompas.Effects.Controllers.Client
{
	public class ClientStackController
	{
		public ClientStackView clientStackPanelCtrl;

		private readonly EffectStack<IClientStackable> stack = new EffectStack<IClientStackable>();

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