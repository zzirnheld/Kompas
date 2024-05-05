using System.Collections.Generic;
using Godot;
using Godot.NativeInterop;
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
			var stackable = IResolvingStackable.Resolving(effect, default(IResolutionContext));
			stack.Push(stackable);
			stackView.Activated(stackable);
		}

		public void Attacked(ClientAttack attack)
		{
			var stackable = IResolvingStackable.Resolving(attack, default(IResolutionContext));
			stack.Push(stackable);
			stackView.Attacked(stackable);
		}

		public void Remove(int index)
		{
			var canceled = stack.Cancel(index);
			if (canceled == null)
			{
				Logger.Err($"Stack index {index} had nothing there");
				return;
			}
			stackView.Cancel(canceled);
		}

		public void Resolve(IClientStackable stackable)
		{
			var topStackable = stack.Pop();
			while (stackable != topStackable?.Stackable && !stack.Empty)
			{
				Logger.Err($"Resolving stackable {stackable} that was not on top. {topStackable} was, instead");
				topStackable = stack.Pop();
			}
			stackView.Resolving(topStackable);
			CurrStackEntry = stackable;
		}

		public void StackEmptied()
		{
			stackView.StackEmptied();
			CurrStackEntry = null;
		}
	}
}