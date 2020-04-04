using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Collisions;


namespace TheTrickster.NPCs {
	public enum TricksterStates : int {
		Idle = 0,
		PreAttack = 1,
		Attack = 2,
		Cooldown = 3
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

			if( this.RunAI_0() ) {
				this.RunAI_1();
			}
		}

		private bool RunAI_0() {
			if( this.State == TricksterStates.Attack ) {
				return true;
			}

			int fleeTicks = TheTricksterConfig.Instance.TicksUntilFlee;
			if( fleeTicks <= 0 ) {
				return true;
			}

			if( this.ElapsedTicksAlive <= fleeTicks ) {
				return true;
			}

			this.FleeAction();
			return false;
		}

		private void RunAI_1() {
			if( this.ElapsedStateTicks < this.GetCurrentStateTickDuration() ) {
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

		private void RunIdleFinishAI() {
			Player player = this.TargetPlayer;
			if( player == null && !player.active && player.dead ) {
				return;
			}

			float distSqr = TheTricksterConfig.Instance.AttackRadius * TheTricksterConfig.Instance.AttackRadius;
			if( Vector2.DistanceSquared( player.Center, this.npc.Center ) >= distSqr ) {
				return;
			}

			this.DodgeAction( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
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
