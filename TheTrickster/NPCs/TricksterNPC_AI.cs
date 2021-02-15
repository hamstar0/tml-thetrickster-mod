using System;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public enum TricksterState : int {
		Idle = 0,
		Mock = 1,
		Lurk = 2,
		PreAttack = 3,
		Attack = 4,
		Cooldown = 5
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

			if( !this.RunAI_Decision_Continuation() ) {
				return;
			}

			if( !this.RunAI_Decision_Action() ) {
				return;
			}

			this.RunAI_States_Finish();
		}


		////

		private bool RunAI_Decision_Continuation() {
			if( this.CanAIContinue( out TricksterDecision decision ) ) {
				return true;
			}

			this.RunAI_Decision( decision );
			return false;
		}

		private bool RunAI_Decision_Action() {
			if( this.CanAIAct( out TricksterDecision decision ) ) {
				return true;
			}

			if( !this.RunAI_Decision( decision ) ) {
				this.RunAI_States();
			}

			return false;
		}

		////

		private bool RunAI_Decision( TricksterDecision decision ) {
			switch( decision ) {
			case TricksterDecision.Flee:
				this.FleeAction( true );
				return true;
				//case TricksterDecision.Laugh:
				//	this.EncounterFX();
				//	return true;
			}
			return false;
		}


		////////////////

		private bool CanAIContinue( out TricksterDecision decision ) {
			if( this.State == TricksterState.Attack ) {
				decision = TricksterDecision.None;
				return true;
			}

			int fleeTicks = TheTricksterConfig.Instance.Get<int>( nameof(TheTricksterConfig.TicksUntilFlee) );
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
			if( this.ElapsedStateTicks < TricksterNPC.GetCurrentStateTickDuration(this.State) ) {
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
	}
}
