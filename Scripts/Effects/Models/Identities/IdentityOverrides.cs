using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities
{
	public class IdentityOverrides
	{
		private readonly Stack<IGameCard> targetOverrides = new();
		public IGameCard TargetCardOverride => targetOverrides.Count > 0 ? targetOverrides.Peek() : null;

		public T WithTargetCardOverride<T> (IGameCard targetOverride, System.Func<T> action)
		{
			targetOverrides.Push(targetOverride);
			var t = action();
			targetOverrides.Pop();
			return t;
		}
	}
}