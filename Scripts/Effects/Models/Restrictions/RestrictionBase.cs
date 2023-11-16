using System;
using Godot;

namespace Kompas.Effects.Models.Restrictions
{
	public abstract class RestrictionBase<RestrictedType> : ContextInitializeableBase, IRestriction<RestrictedType>
	{
		protected virtual bool AllowNullItem => false;

		public bool IsValid(RestrictedType? item, IResolutionContext context)
		{
			ComplainIfNotInitialized();

			try
			{
				if (item == null && !AllowNullItem) return false;
				return IsValidLogic(item, context);
			}
			catch (SystemException exception)
				when (exception is NullReferenceException || exception is ArgumentException)
			{
				GD.PrintErr(exception);
				return false;
			}
		}

		protected abstract bool IsValidLogic(RestrictedType? item, IResolutionContext context);
	}
}