using System.Collections.Generic;
using Godot;
using Kompas.Client.Effects.Models;
using Kompas.Client.Effects.Views;
using Kompas.Effects;
using Kompas.Effects.Models;
using Kompas.Gamestate;

namespace Kompas.Client.Effects.Controllers
{
	public class ClientStackController : IStackController
	{
		private readonly ClientStackView stackView;
		private readonly EffectStack<IClientStackable, IResolutionContext> stack = new();

		public IEnumerable<IClientStackable> StackEntries => stack.StackEntries;
		IEnumerable<IStackable> IStackController.StackEntries => StackEntries;

		public IClientStackable? CurrStackEntry { get; private set; }
		IStackable? IStackController.CurrStackEntry => CurrStackEntry;

		public bool NothingHappening => CurrStackEntry == null;

		public ClientStackController(ClientStackView stackView)
		{
			this.stackView = stackView;
		}

		public void Activated(ClientEffect effect)
		{
			effect.IncrementUses();
			Add(effect);
			stackView.Activated(effect);
		}

		private void Add(IClientStackable stackable, IResolutionContext? context = default)
		{
			stack.Push((stackable, context));
		}

		public void Remove(int index)
		{
			stack.Cancel(index);
		}

		public void Resolve(IClientStackable stackable)
		{
			var (topStackable, _) = stack.Pop();
			while (stackable != topStackable && !stack.Empty)
			{
				Logger.Err($"Resolving stackable {stackable} that was not on top. {topStackable} was, instead");
				(topStackable, _) = stack.Pop();
			}
			stackView.Resolving(stackable);
		}
	}
}