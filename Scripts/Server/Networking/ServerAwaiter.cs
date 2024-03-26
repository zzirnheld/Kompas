using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;

namespace Kompas.Server.Networking
{
	public class ServerAwaiter
	{
		private const int DefaultDelay = 100;
		private const int TargetCheckDelay = 100;

		private readonly ServerGame game;

		public ServerAwaiter(ServerGame game)
		{
			this.game = game;
		}

		#region awaited values
		/*the following are properties so that later i can override the setter to 
		 * also signal a semaphore to awaken the relevant awaiting request. */
		public bool? OptionalTriggerAnswer { get; set; }
		public (int[] cardIds, int[] effIndices, int[] orders)? TriggerOrders { get; set; }

		public int? EffOption { get; set; }
		public int? PlayerXChoice { get; set; }


		//TODO also make sure that when decline target is set, it also wakes up relevant targeting semaphores
		public bool DeclineTarget { get; set; }
		public GameCard? CardTarget { get; set; }
		private IEnumerable<GameCard>? cardListTargets;
		public IEnumerable<GameCard>? CardListTargets
		{
			get => cardListTargets;
			set
			{
				cardListTargets = value;
				//Logger.Log($"Card list targets set to {(value == null ? "null" : string.Join(", ", value))}");
			}
		}
		public (int, int)? SpaceTarget { get; set; }

		public int[]? HandSizeChoices { get; set; }
		#endregion awaited values

		#region trigger things
		public async Task<bool> GetOptionalTriggerChoice(IPlayer toAsk, ServerTrigger trigger, int x, bool showX)
		{
			ServerNotifier.AskForTrigger(toAsk, trigger, x, showX);
			//TODO later use a semaphore or something to signal once packet is available
			while (true)
			{
				if (OptionalTriggerAnswer.HasValue)
				{
					bool answer = OptionalTriggerAnswer.Value;
					OptionalTriggerAnswer = null;
					return answer;
				}

				await Task.Delay(DefaultDelay);
			}
		}

		public async Task GetTriggerOrder(IPlayer toAsk, IEnumerable<ServerTrigger> triggers)
		{
			ServerNotifier.GetTriggerOrder(toAsk, triggers);
			while (true)
			{
				if (TriggerOrders.HasValue)
				{
					(int[] cardIds, int[] effIndices, int[] orders) = TriggerOrders.Value;
					for (int i = 0; i < effIndices.Length; i++)
					{
						//TODO deal with garbage values here
						var card = game.LookupCardByID(cardIds[i]);
						if (card == null) continue;
						if (card.Effects.ElementAt(effIndices[i]).Trigger is ServerTrigger trigger)
							trigger.Order = orders[i];
					}

					TriggerOrders = null;
					return;
				}

				await Task.Delay(DefaultDelay);
			}
		}
		#endregion trigger things

		#region effect flow control
		public async Task<int> GetEffectOption(IPlayer toAsk, string cardName, string choiceBlurb, string[] optionBlurbs, bool hasDefault, bool showX, int x)
		{
			ServerNotifier.ChooseEffectOption(toAsk, cardName, choiceBlurb, optionBlurbs, hasDefault, showX, x);
			while (true)
			{
				if (EffOption.HasValue)
				{
					int val = EffOption.Value;
					EffOption = null;
					return val;
				}

				await Task.Delay(DefaultDelay);
			}
		}

		public async Task<int> GetPlayerXValue(IPlayer toAsk)
		{
			ServerNotifier.GetXForEffect(toAsk);
			while (true)
			{
				if (PlayerXChoice.HasValue)
				{
					int val = PlayerXChoice.Value;
					PlayerXChoice = null;
					return val;
				}

				await Task.Delay(DefaultDelay);
			}
		}
		#endregion effect flow control

		#region targeting
		/// <summary>
		/// Gets a list of card targets, waiting until the client sends one or sends that they don't want to choose targets
		/// </summary>
		/// <param name="sourceCardName">The card whose effect is asking for targets</param>
		/// <param name="blurb">The description of the targets to get</param>
		/// <param name="ids">The list of card ids of valid targets</param>
		/// <param name="listRestrictionJson">The list resriction, if any</param>
		/// <returns>The cards the person chose and false if they chose targets;<br></br>
		/// null and true if they declined to choose targets</returns>
		public async Task<IEnumerable<GameCard>?> GetCardListTargets
			(IPlayer toAsk, string sourceCardName, string blurb, int[] ids, IListRestriction listRestriction)
		{
			ServerNotifier.GetCardTarget(toAsk, sourceCardName, blurb, ids, listRestriction);
			while (true)
			{
				if (CardListTargets != null)
				{
					var targets = CardListTargets;
					CardListTargets = null;
					return targets;
				} //TODO throw exception if there's a problem translating the ids, etc. into targets
				else if (DeclineTarget)
				{
					DeclineTarget = false;
					return null;
				}

				await Task.Delay(TargetCheckDelay);
			}
		}

		/// <summary>
		/// Gets a space target, waiting until the client sends one or send that they don't want to choose a space.
		/// </summary>
		/// <param name="cardName">The card whose effect is asking for a space</param>
		/// <param name="blurb">The description of the space to target</param>
		/// <param name="spaces">The list of valid spaces</param>
		/// <returns>The space and false if the player chose a space<br></br>
		/// default and true if the player declined to choose a space</returns>
		public async Task<Space> GetSpaceTarget
			(IPlayer toAsk, string cardName, string blurb, (int, int)[] spaces, (int, int)[] recommendedSpaces)
		{
			ServerNotifier.GetSpaceTarget(toAsk, cardName, blurb, spaces, recommendedSpaces);
			while (true)
			{
				if (SpaceTarget.HasValue)
				{
					var space = SpaceTarget.Value;
					SpaceTarget = null;
					return space;
				}
				else if (DeclineTarget)
				{
					DeclineTarget = false;
					return Space.Invalid;
				}

				await Task.Delay(TargetCheckDelay);
			}
		}
		#endregion targeting

		#region misc get stuff
		public async Task<int[]> GetHandSizeChoices(IPlayer toAsk, int[] cardIds, string listRestrictionJson)
		{
			ServerNotifier.GetHandSizeChoices(toAsk, cardIds, listRestrictionJson);
			while (true)
			{
				if (HandSizeChoices != null)
				{
					var choices = HandSizeChoices;
					HandSizeChoices = null;
					return choices;
				}

				await Task.Delay(TargetCheckDelay);
			}
		}
		#endregion misc get stuff
	}
}