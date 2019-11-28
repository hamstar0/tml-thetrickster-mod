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
				return TricksterNPC.IdleDurationTicks;
			case TricksterStates.Attack:
				return TricksterNPC.AttackDurationTicks;
			case TricksterStates.Cooldown:
				return TricksterNPC.CooldownDurationTicks;
			}
		}

		////////////////

		public void SetState( TricksterStates newState ) {
			TricksterStates oldState = this.State;
			this.State = newState;

			this.ElapsedStateTicks = 0;
		}


		////////////////

		private void RunAI() {
			this.ElapsedTicksAlive++;
			this.ElapsedStateTicks++;

			int fleeTicks = TheTricksterConfig.Instance.TicksUntilFlee;
			if( fleeTicks > 0 ) {
				if( this.State != TricksterStates.Attack ) {
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

			float distSqr = TricksterNPC.AttackRadius * TricksterNPC.AttackRadius;
			if( Vector2.DistanceSquared( player.Center, this.npc.Center ) >= distSqr ) {
				return;
			}

			this.Dodge( TricksterNPC.DodgeRadius );
			this.SetState( TricksterStates.Attack );
		}

		private void RunAttackFinishAI() {
			this.LaunchAttack();
			this.SetState( TricksterStates.Cooldown );
		}

		private void RunCooldownFinishAI() {
			this.Dodge( TricksterNPC.DodgeRadius );
			this.SetState( TricksterStates.Attack );
		}

		////

		private void RunOnHitAI() {
			if( this.State == TricksterStates.Idle ) {
				return;
			}

			this.Dodge( TricksterNPC.DodgeRadius );
			this.SetState( TricksterStates.Idle );
		}
	}
}
