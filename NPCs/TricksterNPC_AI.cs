using HamstarHelpers.Helpers.Collisions;
using HamstarHelpers.Services.Timers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


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
				return TheTricksterConfig.Instance.AttackDurationTicks;
			case TricksterStates.Cooldown:
				return TheTricksterConfig.Instance.CooldownDurationTicks;
			}
		}

		////////////////

		public void SetState( TricksterStates newState ) {
			this.State = newState;

			this.HitsDuringCurrentStage = 0;
			this.ElapsedStateTicks = 0;

			switch( newState ) {
			case TricksterStates.PreAttack:
				Player plr = this.TargetPlayer;
				if( plr == null ) {
					break;
				}

				(int x, int y) plrTile = ((int)plr.Center.X / 16, (int)plr.Center.Y / 16);
				(int x, int y) npcTile = ((int)this.npc.Center.X / 16, (int)this.npc.Center.Y / 16);
				IList<(int, int)> path;

				if( TileCollisionHelpers.FindPathSimple(plrTile, npcTile, 2000, out path) ) {
					this.SetState( TricksterStates.Attack );
				}
				break;
			}
		}


		////////////////

		private void RunAI() {
			this.ElapsedTicksAlive++;
			this.ElapsedStateTicks++;

			if( this.State != TricksterStates.Attack ) {
				int fleeTicks = TheTricksterConfig.Instance.TicksUntilFlee;
				if( fleeTicks > 0 ) {
					if( this.ElapsedTicksAlive > fleeTicks ) {
						this.Flee();
						return;
					}
				}
			}

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

			this.Dodge( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
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
			this.Dodge( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
			this.SetState( TricksterStates.PreAttack );
		}

		////

		private void RunOnHitAI() {
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

			if( this.HitsDuringCurrentStage++ >= TheTricksterConfig.Instance.HitsBeforeBlink ) {
				this.Dodge( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
				this.SetState( TricksterStates.Idle );
			} else {
				this.ElapsedStateTicks = 1;
			}
		}
	}
}
