using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private bool RunAIDecision( TricksterDecision action ) {
			switch( action ) {
			case TricksterDecision.Flee:
				this.FleeAction();
				return true;
			default:
				return false;
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
			this.SetState( TricksterState.PreAttack );
		}

		private void RunPreAttackFinishAI() {
			this.RunIdleFinishAI();
		}

		private void RunAttackFinishAI() {
			this.LaunchAttack();
			this.SetState( TricksterState.Cooldown );
		}

		private void RunCooldownFinishAI() {
			this.DodgeAction( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
			this.SetState( TricksterState.PreAttack );
		}

		////

		private bool RunOnHitAI() {
			switch( this.State ) {
			case TricksterState.Idle:
				break;
			case TricksterState.PreAttack:
			case TricksterState.Attack:
				this.SetState( TricksterState.Idle );
				break;
			case TricksterState.Cooldown:
				break;
			}

			bool tooManyHits = this.HitsDuringCurrentStage++ >= TheTricksterConfig.Instance.HitsBeforeBlink;

			if( tooManyHits ) {
				this.DeployDefenseBats();
				this.DodgeAction( TheTricksterConfig.Instance.MinDodgeRadius, TheTricksterConfig.Instance.MaxDodgeRadius );
				this.SetState( TricksterState.Idle );
			} else {
				this.ElapsedStateTicks = 1;
			}

			return tooManyHits;
		}
	}
}
