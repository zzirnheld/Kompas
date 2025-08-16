namespace Kompas.Effects.Subeffects;

public abstract class SubeffectData
{
	public bool forbidNotBoard = true;

	/// <summary>
	/// The index in the card targets list for which target this effect uses.
	/// If positive, just an index.
	/// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
	/// </summary>
	public int targetIndex = -1;

	/// <summary>
	/// The index in the space targets list that this subeffect uses.
	/// If positive, just an index.
	/// If negative, it's Count + targetIndex (aka that many back)
	/// </summary>
	public int spaceIndex = -1;

	/// <summary>
	/// The index in the card info targets list for which target this effect uses.
	/// If positive, just an index.
	/// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
	/// </summary>
	public int cardInfoIndex = -1;

	/// <summary>
	/// The index of player in the player targets list
	/// </summary>
	public int playerIndex = -1;

	/// <summary>
	/// The index of the stackable in the stackable targets list
	/// </summary>
	public int stackableIndex = -1;

	/// <summary>
	/// Index for the subeffect to jump to, if it's not going to the next one for some reason
	/// </summary>
	public int[]? jumpIndices;

	/// <summary>
	/// Which of the jump indices to jump to.
	/// Same +- rules as the target/space indices
	/// </summary>
	public int jumpIndicesIndex = -1;

	/// <summary>
	/// If the effect uses X, this is the multiplier to X. Default: 1
	/// </summary>
	public int xMultiplier = 1;

	/// <summary>
	/// If the effect uses X, this is the divisor to X. Default: 1
	/// </summary>
	public int xDivisor = 1;

	/// <summary>
	/// If the effect uses X, this is the modifier to X. Default: 0
	/// </summary>
	public int xModifier = 0;
}