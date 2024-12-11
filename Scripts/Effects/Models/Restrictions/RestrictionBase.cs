using System;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Restrictions;

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
			Logger.Err(exception);
			return false;
		}
		catch (KompasException exception)
		{
			Logger.Err(exception);
			return false;
		}
	}

	protected abstract bool IsValidLogic(RestrictedType? item, IResolutionContext context);
}