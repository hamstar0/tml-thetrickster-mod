using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Collisions;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public enum TricksterStates : int {
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
			case TricksterStates.Idle:
				return TheTricksterConfig.Instance.IdleDurationTicks;
			case TricksterStates.PreAttack:
				return TheTricksterConfig.Instance.IdleDurationTicks;
			case TricksterStates.Attack:
				var myworld = ModContent.GetInstance<TheTricksterWorld>();
				int reducedChargeTime = myworld.TricksterDefeats * TheTricksterConfig.Instance.AttackDurationTicksReducedPerDefeat;
				int chargeTime = TheTricksterConfig.Instance.AttackDurationTicks - reducedChargeTime;
				
				return Math.Max( chargeTime, TheTricksterConfig.Instance.AttackDurationTicksMinimum );
			case TricksterStates.Cooldown:
				return TheTricksterConfig.Instance.CooldownDurationTicks;
			}
		}

		////////////////

		public void SetState( TricksterStates newState, bool syncs=true ) {
			this.State = newState;

			this.HitsDuringCurrentStage = 0;
			this.ElapsedStateTicks = 0;

			switch( newState ) {
			case TricksterStates.PreAttack:
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
				this.SetState( TricksterStates.Attack, false );
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
			case TricksterStates.Idle:
				this.RunIdleFinishAI();
				break;
			case TricksterStates.PreAttack:
				this.RunPreAttackFinishAI();
				break;
			case TricksterStates.Attack:
				this.RunAttackFinishAI();
				break;
			case TricksterStates.Cooldown:
				this.RunCooldownFinishAI();
				break;
			}
		}

		////

		private bool CanAIContinue( out TricksterAction action ) {
			if( this.State == TricksterStates.Attack ) {
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


		////////////////
		
		private void RunAIAction( TricksterAction action ) {
			switch( action ) {
			case TricksterAction.Attack:
				this.AttackChargeSideBehaviors();
				break;
			case TricksterAction.Flee:
				this.FleeAction();
				break;
			}
		}


		////////////////

		private void RunIdleFinishAI() {
			Player player = this.TargetPlayer;
			if( player == null && !player.active && player.dead ) {
				return;
			}

			TheTricksterConfig config = TheTricksterConfig.Instance;
			float distSqr = config.AttackRadius * config.AttackRadius;
			if( Vector2.DistanceSquared( player.Center, this.npc.Center ) >= distSqr ) {
				return;
			}

			this.DodgeAction( config.MinDodgeRadius, config.MaxDodgeRadius );
			this.SetState( TricksterStates.PreAttack );
		}

		private void RunPreAttackFinishAI() {
			this.RunIdleFinishAI();
		}

		private void RunAttackFinishAI() {
			this.LaunchAttack();
			this.SetState( TricksterStates.Cooldown );
		}

		private void RunCooldownFinishAI() {
			this.DodgeAction( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
			this.SetState( TricksterStates.PreAttack );
		}

		////

		private bool RunOnHitAI() {
			switch( this.State ) {
			case TricksterStates.Idle:
				break;
			case TricksterStates.PreAttack:
			case TricksterStates.Attack:
				this.SetState( TricksterStates.Idle );
				break;
			case TricksterStates.Cooldown:
				break;
			}

			bool tooManyHits = this.HitsDuringCurrentStage++ >= TheTricksterConfig.Instance.HitsBeforeBlink;

			if( tooManyHits ) {
				this.DodgeAction( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
				this.SetState( TricksterStates.Idle );
			} else {
				this.ElapsedStateTicks = 1;
			}

			return tooManyHits;
		}
	}
}
