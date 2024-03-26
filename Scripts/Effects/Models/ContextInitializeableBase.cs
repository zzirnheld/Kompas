using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Godot;
using Kompas.Effects.Models.InitializationRequirements;

namespace Kompas.Effects.Models
{
	/// <summary>
	/// Base class for initializeable things, like restrictions or identities.
	/// Since these are all being loaded from JSON, make sure to mark any relevant fields as [JsonProperty]
	/// </summary>
	[DataContract]
	public abstract class ContextInitializeableBase : IContextInitializeable
	{
		protected bool Initialized { get; private set; }

		protected InitializationContext InitializationContext { get; private set; }

		protected virtual IEnumerable<IInitializationRequirement> InitializationRequirements => Enumerable.Empty<IInitializationRequirement>();

		public virtual void Initialize(InitializationContext initializationContext)
		{
			if (Initialized)
			{
				Logger.Log($"Was already initialized with {InitializationContext}, but now being initialized with {initializationContext}");
			}
			InitializationContext = initializationContext;

			Initialized = true;
		}

		protected virtual void ComplainIfNotInitialized()
		{
			if (!Initialized) throw new System.NotImplementedException($"You forgot to initialize a {GetType()}!\n{this}");
		}
		
		protected static bool AllNull(params object?[] objs) => objs.All(o => o == null);
		protected static bool MultipleNonNull(params object?[] objs) => objs.Count(o => o != null) > 1;

		public override string ToString()
		{
			return GetType().ToString();
		}

		public virtual void AdjustSubeffectIndices(int increment, int startingAtIndex = 0) { }

		public static void AdjustSubeffectIndices(int[]? subeffectIndices, int increment, int startingAtIndex)
		{
			if (subeffectIndices == null) return;

			for (int i = 0; i < subeffectIndices.Length; i++)
				if (subeffectIndices[i] >= startingAtIndex)
					subeffectIndices[i] += increment;
		}
	}
}