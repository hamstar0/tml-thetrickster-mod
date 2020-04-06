using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private void RunLurkPassiveAI() {
			var config = TheTricksterConfig.Instance;
			int plrs = Main.player.Length;
			float stealRangeSqr = config.LurkStealRange * config.LurkStealRange;

			bool isThief = false;

			for( int i=0; i<plrs; i++ ) {
				Player plr = Main.player[i];
				if( plr?.active != true || plr.dead ) {
					continue;
				}

				if( (plr.Center - this.npc.Center).LengthSquared() >= stealRangeSqr ) {
					continue;
				}

				isThief = this.StealFromPlayer( plr );
				break;
			}

			if( isThief ) {
				this.DodgeAction( config.MinDodgeRadius, config.MaxDodgeRadius );
				this.SetState( TricksterState.PreAttack );
			}
		}


		////////////////

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


		private void RunLurkFinishAI() {
			Player player = this.TargetPlayer;
			if( player == null && !player.active && player.dead ) {
				return;
			}

			TheTricksterConfig config = TheTricksterConfig.Instance;
			float distSqr = config.AttackRadius * config.AttackRadius;
			if( Vector2.DistanceSquared( player.Center, this.npc.Center ) >= distSqr ) {
				return;
			}

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
			var config = TheTricksterConfig.Instance;

			this.DodgeAction( config.MinDodgeRadius, config.MaxDodgeRadius );
			this.SetState( TricksterState.PreAttack );
		}


		////

		private bool RunOnHitAI() {
			var config = TheTricksterConfig.Instance;

			switch( this.State ) {
			case TricksterState.Idle:
			case TricksterState.Lurk:
				break;
			case TricksterState.PreAttack:
			case TricksterState.Attack:
				this.SetState( TricksterState.Idle );
				break;
			case TricksterState.Cooldown:
				break;
			}

			bool tooManyHits = this.HitsDuringCurrentStage++ >= config.HitsBeforeBlink;

			if( tooManyHits ) {
				this.DeployDefenseBats();
				this.DodgeAction( config.MinDodgeRadius, config.MaxDodgeRadius );
				this.SetState( TricksterState.Idle );
			} else {
				this.ElapsedStateTicks = 1;
			}

			return tooManyHits;
		}
	}
}
