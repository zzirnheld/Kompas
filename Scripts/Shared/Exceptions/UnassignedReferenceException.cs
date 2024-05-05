using System;

namespace Kompas.Shared.Exceptions
{
	public class UnassignedReferenceException
		: Exception
	{
		private readonly string? fieldName;

		public UnassignedReferenceException()
		{

		}

		public UnassignedReferenceException(string fieldName)
		{
			this.fieldName = fieldName;
		}

        public override string ToString()
        {
            return $"{base.ToString()} {fieldName}";
        }
    }
}