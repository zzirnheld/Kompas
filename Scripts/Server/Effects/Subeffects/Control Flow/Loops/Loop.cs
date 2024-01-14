using System.Threading.Tasks;
using Godot;
using Kompas.Gamestate.Exceptions;
using Kompas.Server.Effects.Models;
using Kompas.Server.Networking;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Loop : ServerSubeffect
	{
		public bool canDecline = false;

		protected virtual void OnLoopExit()
		{
			//make the "no other targets" button disappear
			if (canDecline)
			{
				var player = PlayerTarget ?? throw new NullPlayerException(TargetWasNull);
				//TODO - do this for both players? in case loop contained something setting target. or maybe store the player that's in a can decline loop?
				ServerNotifier.DisableDecliningTarget(player);
				ServerNotifier.AcceptTarget(player); // otherwise it keeps them in the now-irrelevant target mode
			}
		}

		protected virtual bool ShouldContinueLoop => true;

		public override Task<ResolutionInfo> Resolve()
		{
			//loop again if necessary
			GD.Print($"im in ur loop of type {GetType()}, the one that jumps to {JumpIndex}");
			if (ShouldContinueLoop)
			{
				//tell the client to enable the button to exit the loop
				if (canDecline)
				{
					var player = PlayerTarget ?? throw new NullPlayerException(TargetWasNull);
					ServerNotifier.EnableDecliningTarget(player);
					ServerEffect.OnImpossible = this;
					ServerEffect.CanDeclineTarget = true;
				}
				return Task.FromResult(ResolutionInfo.Index(JumpIndex));
			}
			else return ExitLoop();
		}

		/// <summary>
		/// Cancels the loop (because the player declined another target, or because there are no more valid targets)
		/// </summary>
		public Task<ResolutionInfo> ExitLoop()
		{
			//let parent know the loop is over
			if (ServerEffect.OnImpossible == this) ServerEffect.OnImpossible = null;
			ServerEffect.CanDeclineTarget = false;

			//do anything necessary to clean up the loop
			OnLoopExit();

			//then skip to after the loop (exitloop will sometimes be called while the effect is waiting on a target,
			//on a subeffect that isn't this one. resolvenext won't work in that situation.
			return Task.FromResult(ResolutionInfo.Index(SubeffIndex + 1));
		}

		public override Task<ResolutionInfo> OnImpossible(string why)
		{
			if (canDecline) return ExitLoop();
			else return base.OnImpossible(why);
		}
	}
}