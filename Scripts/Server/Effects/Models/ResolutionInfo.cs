namespace Kompas.Server.Effects.Models;

public struct ResolutionInfo
{
	public const string EndedBecauseImpossible = "Ended because effect was impossible";

	public ResolutionResult result;

	public int index;

	public string reason;

	public static ResolutionInfo Next => new ResolutionInfo { result = ResolutionResult.Next };
	public static ResolutionInfo Index(int index) => new ResolutionInfo { result = ResolutionResult.Index, index = index };
	public static ResolutionInfo Impossible(string why) => new ResolutionInfo { result = ResolutionResult.Impossible, reason = why };
	public static ResolutionInfo End(string why) => new ResolutionInfo { result = ResolutionResult.End, reason = why };
}
