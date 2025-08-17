using System.Collections.Generic;
using Kompas.Effects.Subeffects;
using Kompas.Server.Effects.Models;
using Kompas.Server.Effects.Models.Subeffects;

namespace Kompas.Server.Effects;

public static class ServerSubeffectFactory
{

	// In theory, I could put a ToServerSubeffect function on SubeffectData to force this to be implemented.
	// That would kinda violate the whole point of separating data to be unrelated to the Server package, tho
	public static ServerSubeffect FromData(SubeffectData subeffectData) => subeffectData switch
	{
		ClearOnImpossibleData data => new ClearOnImpossible(data),
		CountXLoopData data => new CountXLoop(data),
		LoopWhileHaveTargetsData data => new LoopWhileHaveTargets(data),
		XTimesData data => new XTimes(data),
		LoopData data => new Loop(data),

		CanResolveData data => new CanResolve(data),
		ChooseOptionData data => new ChooseOption(data),
		ConditionalEndData data => new ConditionalEnd(data),
		ConditionalJumpData data => new ConditionalJump(data),
		DisableDecliningTargetData data => new DisableDecliningTarget(data),
		EnableDecliningTargetData data => new EnableDecliningTarget(data),
		EndResolutionData data => new EndResolution(data),
		JumpData data => new Jump(data),
		SkipToEffectOnImpossibleData data => new SkipToEffectOnImpossible(data),

		Models.Subeffects.Hanging.ActivationData data => new Models.Subeffects.Hanging.Activation(data),
		Models.Subeffects.Hanging.AnnihilationData data => new Models.Subeffects.Hanging.Annihilation(data),
		Models.Subeffects.Hanging.ChangeAllCardStatsData data => new Models.Subeffects.Hanging.ChangeAllCardStats(data),
		Models.Subeffects.Hanging.ChangeCardStatsData data => new Models.Subeffects.Hanging.ChangeCardStats(data),
		Models.Subeffects.Hanging.DelayData data => new Models.Subeffects.Hanging.Delay(data),
		Models.Subeffects.Hanging.DiscardData data => new Models.Subeffects.Hanging.Discard(data),
		Models.Subeffects.Hanging.NegateData data => new Models.Subeffects.Hanging.Negate(data),

		VanishData data => new Vanish(data),
		AnnihilateData data => new Annihilate(data),
		DiscardData data => new Discard(data),
		HandData data => new Hand(data),
		PlayData data => new Play(data),
		ReshuffleData data => new Reshuffle(data),
		TopdeckData data => new Topdeck(data),
		BottomdeckData data => new Bottomdeck(data),

		AttachData data => new Attach(data),
		BottomdeckRestData data => new BottomdeckRest(data),
		DrawData data => new Draw(data),
		DrawXData data => new DrawX(data),
		MillData data => new Mill(data),
		MoveData data => new Move(data),
		RevealData data => new Reveal(data),
		ShowData data => new Show(data),
		ShuffleDeckData data => new ShuffleDeck(data),
		SwapData data => new Swap(data),

		AttackData data => new Attack(data),
		ChangeLeyloadData data => new ChangeLeyload(data),
		EndTurnData data => new EndTurn(data),
		KeywordData data => new Keyword(data),
		TakeControlData data => new TakeControl(data),
		UnlinkCardsData data => new UnlinkCards(data),

		ChangeAllCardStatsData data => new ChangeAllCardStats(data),
		PayStatsData data => new PayStats(data),
		ChangeCardStatsData data => new ChangeCardStats(data),
		DamageData data => new Damage(data),
		HealData data => new Heal(data),
		ResetStatsData data => new ResetStats(data),
		SetCardStatsData data => new SetCardStats(data),
		SpendRemainingMovementData data => new SpendRemainingMovement(data),
		SwapNESWData data => new SwapNESW(data),
		SwapStatData data => new SwapStat(data),

		PayPipsData data => new PayPips(data),
		PayPipsTargetCostData data => new PayPipsTargetCost(data),

		ActivateData data => new Activate(data),
		AddPipsData data => new AddPips(data),
		DispelData data => new Dispel(data),
		NegateData data => new Negate(data),
		ResummonAllData data => new ResummonAll(data),
		ResummonData data => new Resummon(data),

		TargetAugmentedCardData data => new TargetAugmentedCard(data),
		TargetAvatarData data => new TargetAvatar(data),
		TargetDefenderData data => new TargetDefender(data),
		TargetOtherInFightData data => new TargetOtherInFight(data),
		TargetTargetsAugmentedCardData data => new TargetTargetsAugmentedCard(data),
		TargetThisData data => new TargetThis(data),

		AutoTargetData data => new AutoTarget(data),
		AutoTargetCardIdentityData data => new AutoTargetCardIdentity(data),
		CardTargetSaveRestData data => new CardTargetSaveRest(data),
		TargetAugmentsData data => new TargetAugments(data),
		TargetAllData data => new TargetAll(data),
		AddRestData data => new AddRest(data),
		CardTargetData data => new CardTarget(data),
		TargetTriggeringCardData data => new TargetTriggeringCard(data),

		TargetEnemyData data => new TargetEnemy(data),
		TargetTargetsControllerData data => new TargetTargetsController(data),
		TargetTriggeringCardsSpaceData data => new TargetTriggeringCardsSpace(data),
		TargetTurnPlayerData data => new TargetTurnPlayer(data),
		
		ClearRestData data => new ClearRest(data),

		AutoSpaceTargetData data => new AutoSpaceTarget(data),
		AutoTargetSpaceIdentityData data => new AutoTargetSpaceIdentity(data),
		SpaceTargetData data => new SpaceTarget(data),
		TargetDirectionData data => new TargetDirection(data),
		TargetDisplacementData data => new TargetDisplacement(data),
		TargetTargetsSpaceData data => new TargetTargetsSpace(data),
		TargetThisSpaceData data => new TargetThisSpace(data),
		TargetTriggeringCoordsData data => new TargetTriggeringCoords(data),

		DeleteTargetData data => new DeleteTarget(data),
		StashTargetsCardInfoData data => new StashTargetsCardInfo(data),

		PlayerChooseXData data => new PlayerChooseX(data),
		SetXByNumberIdentityData data => new SetXByNumberIdentity(data),
		SetXData data => new SetX(data),

		_ => throw new System.NotImplementedException(subeffectData.GetType().ToString()),
	};
}