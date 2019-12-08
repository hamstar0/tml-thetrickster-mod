using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public enum TricksterStates : int {
		Idle = 0,
		Attack = 1,
		Cooldown = 2
	}




	partial class TricksterNPC : ModNPC {
		public int GetCurrentStateTickDuration() {
			switch( this.State ) {
			default:
			case TricksterStates.Idle:
				return TheTricksterConfig.Instance.IdleDurationTicks;
			case TricksterStates.Attack:
				return TheTricksterConfig.Instance.AttackDurationTicks;
			case TricksterStates.Cooldown:
				return TheTricksterConfig.Instance.CooldownDurationTicks;
			}
		}

		////////////////

		public void SetState( TricksterStates newState ) {
			TricksterStates oldState = this.State;
			this.State = newState;

			this.IdleHits = 0;
			this.ElapsedStateTicks = 0;
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

			this.Dodge( TheTricksterConfig.Instance.DodgeRadius );
			this.SetState( TricksterStates.Attack );
		}

		private void RunAttackFinishAI() {
			this.LaunchAttack();
			this.SetState( TricksterStates.Cooldown );
		}

		private void RunCooldownFinishAI() {
			this.Dodge( TheTricksterConfig.Instance.DodgeRadius );
			this.SetState( TricksterStates.Attack );
		}

		////

		private void RunOnHitAI() {
			if( this.State == TricksterStates.Idle ) {
				if( this.IdleHits++ < 3 ) {
					return;
				}
			}

			this.Dodge( TheTricksterConfig.Instance.DodgeRadius );
			this.SetState( TricksterStates.Idle );
		}
	}
}
