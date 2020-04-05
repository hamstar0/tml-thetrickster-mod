using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Collisions;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public enum TricksterState : int {
		Idle = 0,
		PreAttack = 1,
		Attack = 2,
		Cooldown = 3
	}


	public enum TricksterAction : int {
		None = 0,
		Attack = 1,
		Flee = 2
	}




	public partial class TricksterNPC : ModNPC {
		public int GetCurrentStateTickDuration() {
			switch( this.State ) {
			default:
			case TricksterState.Idle:
				return TheTricksterConfig.Instance.IdleDurationTicks;
			case TricksterState.PreAttack:
				return TheTricksterConfig.Instance.IdleDurationTicks;
			case TricksterState.Attack:
				var myworld = ModContent.GetInstance<TheTricksterWorld>();
				int reducedChargeTime = myworld.TricksterDefeats * TheTricksterConfig.Instance.AttackDurationTicksReducedPerDefeat;
				int chargeTime = TheTricksterConfig.Instance.AttackDurationTicks - reducedChargeTime;
				
				return Math.Max( chargeTime, TheTricksterConfig.Instance.AttackDurationTicksMinimum );
			case TricksterState.Cooldown:
				return TheTricksterConfig.Instance.CooldownDurationTicks;
			}
		}

		////////////////

		public void SetState( TricksterState newState, bool syncs=true ) {
			this.State = newState;

			this.HitsDuringCurrentStage = 0;
			this.ElapsedStateTicks = 0;

			switch( newState ) {
			case TricksterState.PreAttack:
				this.ApplyPreAttackState();
				break;
			}

			if( syncs && Main.netMode == 2 ) {
				TricksterStateProtocol.Broadcast( this.npc.whoAmI, newState );
			}
		}

		////

		private void ApplyPreAttackState() {
			Player plr = this.TargetPlayer;
			if( plr == null ) {
				return;
			}

			(int x, int y) plrTile = ((int)plr.Center.X >> 4, (int)plr.Center.Y >> 4);
			(int x, int y) npcTile = ((int)this.npc.Center.X >> 4, (int)this.npc.Center.Y >> 4);
			IList<(int, int)> path;

			if( TileCollisionHelpers.FindPathSimple(plrTile, npcTile, 2000, out path) ) {
				this.SetState( TricksterState.Attack, false );
			}
		}


		////////////////

		private void RunAI() {
			this.ElapsedTicksAlive++;
			this.ElapsedStateTicks++;

			TricksterAction action;

			if( !this.CanAIContinue(out action) ) {
				this.RunAIAction( action );
				return;
			}
			if( !this.CanAIAct(out action) ) {
				this.RunAIAction( action );
				return;
			}

			switch( this.State ) {
			case TricksterState.Idle:
				this.RunIdleFinishAI();
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
			}
		}

		////

		private bool CanAIContinue( out TricksterAction action ) {
			if( this.State == TricksterState.Attack ) {
				action = TricksterAction.None;
				return true;
			}

			int fleeTicks = TheTricksterConfig.Instance.TicksUntilFlee;
			if( fleeTicks <= 0 ) {
				action = TricksterAction.None;
				return true;
			}

			if( this.ElapsedTicksAlive <= fleeTicks ) {
				action = TricksterAction.None;
				return true;
			}

			action = TricksterAction.Flee;
			return false;
		}

		private bool CanAIAct( out TricksterAction action ) {
			if( this.ElapsedStateTicks < this.GetCurrentStateTickDuration() ) {
				action = TricksterAction.None;
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

			action = TricksterAction.None;
			return true;
		}
	}
}
