using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ConditionalJumpData : SubeffectData
{
	[JsonProperty(Required = Required.Always)]
	public IGamestateRestriction? jumpIfTrue;
}

public class ConditionalJump : ServerSubeffect<ConditionalJumpData>
{
	private readonly IGamestateRestriction jumpIfTrue;

	public ConditionalJump(ConditionalJumpData data) : base(data)
	{
		jumpIfTrue = data.jumpIfTrue ?? throw new MissingJSONValueException(nameof(jumpIfTrue), this);
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		jumpIfTrue.Initialize(DefaultInitializationContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (jumpIfTrue.IsValid(resolution.Context)) return Task.FromResult(ResolutionInfo.Index(JumpIndex));
		else return Task.FromResult(ResolutionInfo.Next);
	}
}