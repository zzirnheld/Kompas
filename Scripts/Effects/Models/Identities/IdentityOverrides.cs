using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities
{
	public class IdentityOverrides
	{
		private readonly Stack<IGameCardInfo> targetOverrides = new();
		public IGameCardInfo TargetCardOverride => targetOverrides.Count > 0 ? targetOverrides.Peek() : null;

		public T WithTargetCardOverride<T> (IGameCardInfo targetOverride, System.Func<T> action)
		{
			targetOverrides.Push(targetOverride);
			var t = action();
			targetOverrides.Pop();
			return t;
		}
	}
}