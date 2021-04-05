using System;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public enum TricksterState : int {
		Idle = 0,
		Mock = 1,
		Lurk = 2,
		PreAttack = 3,
		AttackChargeup = 4,
		Cooldown = 5
	}


	public enum TricksterDecision : int {
		None = 0,
		Flee = 1,
		StageFinished = 2,
		//Laugh = 3
	}




	public partial class TricksterNPC : ModNPC {
		private void RunAI() {
			this.ElapsedTicksAlive++;
			this.ElapsedStateTicks++;

			TricksterDecision decision = this.GetAIDecision();

			if( decision != TricksterDecision.None ) {
				this.EnactAIDecision( decision );
			} else {
				this.RunAI_States();
			}
		}


		////////////////

		private TricksterDecision GetAIDecision() {
			if( this.State != TricksterState.AttackChargeup ) {
				var config = TheTricksterConfig.Instance;
				int fleeTicks = config.Get<int>( nameof(config.MaxEncounterDurationTicks) );

				if( fleeTicks > 0 && this.ElapsedTicksAlive > fleeTicks ) {
					return TricksterDecision.Flee;
				}

			}

			int stageDuration = TricksterNPC.GetCurrentStateTickDuration( this.State );
			if( this.ElapsedStateTicks >= stageDuration ) {
				return TricksterDecision.StageFinished;
			}

			/*string timerName = "TricksterProximityScan_" + this.npc.whoAmI;
			int time = Timers.GetTimerTickDuration( timerName );

			if( time > 0 ) {
				Timers.SetTimer( timerName, 30, false, () => false );

				IList<int> npcWhos = NPCFinderHelpers.FindNPCsNearby( this.npc.Center, 0, 16 * 64, false );

				if( npcWhos.Count < TheTricksterConfig.Instance.MaximumNearbyMobsBeforeFleeing ) {
					return TricksterAction.Flee;
				}
			}*/

			return TricksterDecision.None;
		}


		////////////////

		private void EnactAIDecision( TricksterDecision decision ) {
			switch( decision ) {
			case TricksterDecision.Flee:
				this.FleeAction( true );
				break;
			case TricksterDecision.StageFinished:
				this.FinishAIState();
				break;
			//case TricksterDecision.Laugh:
			//	this.EncounterFX();
			//	return true;
			}
		}
	}
}
