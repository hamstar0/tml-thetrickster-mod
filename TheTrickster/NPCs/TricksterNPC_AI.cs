using System;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public enum TricksterState : int {
		Idle = 0,
		Lurk = 1,
		PreAttack = 2,
		Attack = 3,
		Cooldown = 4
	}


	public enum TricksterDecision : int {
		None = 0,
		Flee = 1,
		//Laugh = 2
	}




	public partial class TricksterNPC : ModNPC {
		private void RunAI() {
			this.ElapsedTicksAlive++;
			this.ElapsedStateTicks++;

			TricksterDecision decision;

			if( !this.CanAIContinue(out decision) ) {
				this.RunAIDecision( decision );
				return;
			}
			if( !this.CanAIAct(out decision) ) {
				if( !this.RunAIDecision(decision) ) {
					this.RunAIPassive();
				}
				return;
			}

			this.RunAIStateFinish();
		}

		////

		private bool CanAIContinue( out TricksterDecision decision ) {
			if( this.State == TricksterState.Attack ) {
				decision = TricksterDecision.None;
				return true;
			}

			int fleeTicks = TheTricksterConfig.Instance.TicksUntilFlee;
			if( fleeTicks <= 0 ) {
				decision = TricksterDecision.None;
				return true;
			}

			if( this.ElapsedTicksAlive <= fleeTicks ) {
				decision = TricksterDecision.None;
				return true;
			}

			decision = TricksterDecision.Flee;
			return false;
		}

		private bool CanAIAct( out TricksterDecision action ) {
			if( this.ElapsedStateTicks < this.GetCurrentStateTickDuration() ) {
				action = TricksterDecision.None;
				return false;
			}

			/*string timerName = "TricksterProximityScan_" + this.npc.whoAmI;
			int time = Timers.GetTimerTickDuration( timerName );

			if( time > 0 ) {
				Timers.SetTimer( timerName, 30, false, () => false );

				IList<int> npcWhos = NPCFinderHelpers.FindNPCsNearby( this.npc.Center, 0, 16 * 64, false );

				if( npcWhos.Count < TheTricksterConfig.Instance.MaximumNearbyMobsBeforeFleeing ) {
					action = TricksterAction.Flee;
					return false;
				}
			}*/

			action = TricksterDecision.None;
			return true;
		}


		////////////////

		private void RunAIPassive() {
			switch( this.State ) {
			case TricksterState.Lurk:
				this.RunLurkPassiveAI();
				break;
			case TricksterState.Attack:
				this.AttackChargingSideEffects();
				break;
			}
		}


		////////////////

		private void RunAIStateFinish() {
			switch( this.State ) {
			case TricksterState.Idle:
				this.RunIdleFinishAI();
				break;
			case TricksterState.Lurk:
				this.RunLurkFinishAI();
				break;
			case TricksterState.PreAttack:
				this.RunPreAttackFinishAI();
				break;
			case TricksterState.Attack:
				this.RunAttackFinishAI();
				break;
			case TricksterState.Cooldown:
				this.RunCooldownFinishAI();
				break;
			default:
				break;
			}
		}
	}
}
