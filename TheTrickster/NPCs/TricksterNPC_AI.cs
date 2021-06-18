using System;
using Terraria;
using Terraria.ID;
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
		FleeNoBombs = 1,
		FleeWithBombs = 2,
		StageFinished = 3,
		//Laugh = 4
	}




	public partial class TricksterNPC : ModNPC {
		private void RunAI() {
			this.ElapsedTicksAlive++;
			this.ElapsedStateTicks++;

			TricksterDecision decision = this.GetGeneralAIDecision();

			if( decision == TricksterDecision.None ) {
				decision = this.RunAI_States();
			}

			if( Main.netMode != NetmodeID.MultiplayerClient ) {
				this.EnactAIDecision( decision );
			}
		}


		////////////////

		private TricksterDecision GetGeneralAIDecision() {
			if( this.State != TricksterState.AttackChargeup ) {
				var config = TheTricksterConfig.Instance;
				int fleeTicks = config.Get<int>( nameof(config.MaxEncounterDurationTicks) );

				if( fleeTicks > 0 && this.ElapsedTicksAlive > fleeTicks ) {
					return TricksterDecision.FleeNoBombs;
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

				IList<int> npcWhos = NPCFinderLibraries.FindNPCsNearby( this.npc.Center, 0, 16 * 64, false );

				if( npcWhos.Count < TheTricksterConfig.Instance.MaximumNearbyMobsBeforeFleeing ) {
					return TricksterAction.Flee;
				}
			}*/

			return TricksterDecision.None;
		}


		////////////////

		public void EnactAIDecision( TricksterDecision decision ) {
			switch( decision ) { f
			case TricksterDecision.FleeNoBombs:
				this.FleeAction( false );
				break;
			case TricksterDecision.FleeWithBombs:
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
