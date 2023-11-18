using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Subeffects
{
	/// <summary>
	/// Not abstract because it's instantiated as part of loading subeffects
	/// </summary>
	public class Subeffect
	{
		#region reasons for impossible
		public const string TargetWasNull = "No target to affect";
		public const string TargetAlreadyThere = "Target was already in the place to move it to";
		public const string NoValidCardTarget = "No valid card to target";
		public const string NoValidSpaceTarget = "No valid space to target";
		public const string ChangedStatsOfCardOffBoard = "Can't change stats of card not on the board";
		public const string MovedCardOffBoard = "Moved card not on the board";
		public const string EndOnPurpose = "Ended early on purpose";

		public const string CantAffordPips = "Can't afford pips";
		public const string CantAffordStats = "Can't afford stats";

		public const string DeclinedFurtherTargets = "Declined further targets";

		//card movement failure
		public const string AnnihilationFailed = "Target couldn't be annihilated";
		public const string AttachFailed = "Attach as augment failed";
		public const string BottomdeckFailed = "Bottomdeck failed";
		public const string DiscardFailed = "Discard failed";
		public const string CouldntDrawAllX = "Couldn't draw all X cards";
		public const string CouldntMillAllX = "Couldn't mill all X cards";
		public const string MovementFailed = "Movement failed";
		public const string PlayingFailed = "Playing card failed";
		public const string RehandFailed = "Rehanding card failed";
		public const string ReshuffleFailed = "Reshuffle failed";
		public const string TopdeckFailed = "Topdeck failed";

		//misc
		public const string TooMuchEForHeal = "Target already has at least their printed E";
		#endregion reasons for impossible

		public class TargetingContext
		{
			public int? cardTargetIndex;
			public int? spaceTargetIndex;
			public int? cardInfoTargetIndex;
			public int? playerTargetIndex;
			public int? stackableTargetIndex;

			public TargetingContext OrElse(TargetingContext other) => new()
			{
				cardTargetIndex = cardTargetIndex ?? other.cardTargetIndex,
				spaceTargetIndex = spaceTargetIndex ?? other.spaceTargetIndex,
				cardInfoTargetIndex = cardInfoTargetIndex ?? other.cardInfoTargetIndex,
				playerTargetIndex = playerTargetIndex ?? other.playerTargetIndex,
				stackableTargetIndex = stackableTargetIndex ?? other.stackableTargetIndex,
			};
		}

		public TargetingContext CurrTargetingContext => new()
		{
			cardTargetIndex = targetIndex,
			spaceTargetIndex = spaceIndex,
			cardInfoTargetIndex = cardInfoIndex,
			playerTargetIndex = playerIndex,
			stackableTargetIndex = stackableIndex
		};

		public virtual Effect? Effect { get; }
		public virtual IGame? Game { get; }

		public int SubeffIndex { get; protected set; }

		public IResolutionContext ResolutionContext
		{
			get
			{
				_ = Effect ?? throw new System.NullReferenceException("Checked resolution context of the subeffect before its effect was assigned!");
				return Effect.CurrentResolutionContext;;
			}
		}

		/// <summary>
		/// Represents the type of subeffect this is
		/// </summary>
		//public string subeffType;

		public bool forbidNotBoard = true;

		#region targeting indices
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
		public int[] jumpIndices;

		/// <summary>
		/// Which of the jump indices to jump to.
		/// Same +- rules as the target/space indices
		/// </summary>
		public int jumpIndicesIndex = -1;
		#endregion targeting indices

		#region effect x
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

		/// <summary>
		/// If the effect uses X, this is the adjusted value of X
		/// </summary>
		public int Count => (Effect.X * xMultiplier / xDivisor) + xModifier;
		#endregion effect x

		public GameCard CardTarget => Effect.GetTarget(targetIndex);
		public Space SpaceTarget => Effect.GetSpace(spaceIndex);
		public GameCardInfo CardInfoTarget => EffectHelper.GetItem(Effect.CardInfoTargets, cardInfoIndex);
		public IPlayer PlayerTarget => Effect.GetPlayer(playerIndex);
		public IStackable StackableTarget => EffectHelper.GetItem(Effect.StackableTargets, stackableIndex);

		public GameCard GetCardTarget(TargetingContext overrideContext = null)
			=> Effect.GetTarget(overrideContext.OrElse(CurrTargetingContext).cardTargetIndex.Value);
		public Space GetSpaceTarget(TargetingContext overrideContext = null)
			=> Effect.GetSpace(overrideContext.OrElse(CurrTargetingContext).spaceTargetIndex.Value);
		public IPlayer GetPlayerTarget(TargetingContext overrideContext = null)
			=> Effect.GetPlayer(overrideContext.OrElse(CurrTargetingContext).playerTargetIndex.Value);
		public IStackable GetStackableTarget(TargetingContext overrideContext = null)
			=> EffectHelper.GetItem(Effect.StackableTargets, overrideContext.OrElse(CurrTargetingContext).stackableTargetIndex.Value);


		public int JumpIndex => EffectHelper.GetItem(jumpIndices, jumpIndicesIndex);

		public void RemoveTarget()
		{
			if (CardTarget == null) throw new NullCardException(TargetWasNull);

			Effect.RemoveTarget(CardTarget);
		}
	}
}