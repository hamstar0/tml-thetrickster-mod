using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private bool RunAIDecision( TricksterDecision decision ) {
			switch( decision ) {
			case TricksterDecision.Flee:
				this.FleeAction();
				return true;
			//case TricksterDecision.Laugh:
			//	this.EncounterFX();
			//	return true;
			}
			return false;
		}


		////////////////

		private void RunLurkPassiveAI() {
			var config = TheTricksterConfig.Instance;
			int lurkStealRange = config.Get<int>( nameof(TheTricksterConfig.LurkStealRange) );
			float stealRangeSqr = lurkStealRange * lurkStealRange;

			bool isThief = false;

			int plrs = Main.player.Length;
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
				int minDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MinDodgeRadius) );
				this.DodgeAction( minDodgeRad, minDodgeRad );
				this.EncounterFormal();
				this.SetState( TricksterState.PreAttack );
			}
		}


		////////////////

		private void RunIdleFinishAI() {
			Player player = this.TargetPlayer;
			if( player == null && !player.active && player.dead ) {
				return;
			}

			var config = TheTricksterConfig.Instance;
			int atkRad = config.Get<int>( nameof(TheTricksterConfig.AttackRadius) );
			float distSqr = atkRad * atkRad;

			if( Vector2.DistanceSquared(player.Center, this.npc.Center) >= distSqr ) {
				return;
			}

			int minDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MinDodgeRadius) );
			int maxDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MaxDodgeRadius) );

			this.DodgeAction( minDodgeRad, maxDodgeRad );
			this.SetState( TricksterState.PreAttack );
		}


		private void RunLurkFinishAI() {
			Player player = this.TargetPlayer;
			if( player == null && !player.active && player.dead ) {
				return;
			}

			var config = TheTricksterConfig.Instance;
			int atkRad = config.Get<int>( nameof(TheTricksterConfig.AttackRadius) );
			float distSqr = atkRad * atkRad;
			if( Vector2.DistanceSquared(player.Center, this.npc.Center) >= distSqr ) {
				return;
			}

			this.EncounterFormal();
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
			int minDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MinDodgeRadius) );
			int maxDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MaxDodgeRadius) );

			this.DodgeAction( minDodgeRad, maxDodgeRad );
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

			int hitsBeforeBlink = config.Get<int>( nameof(TheTricksterConfig.HitsBeforeBlink) );
			bool tooManyHits = this.HitsDuringCurrentStage++ >= hitsBeforeBlink;

			if( tooManyHits ) {
				int minDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MinDodgeRadius) );
				int maxDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MaxDodgeRadius) );

				this.DeployDefenseBats();
				this.DodgeAction( minDodgeRad, maxDodgeRad );
				this.SetState( TricksterState.Idle );
			} else {
				this.ElapsedStateTicks = 1;
			}

			return tooManyHits;
		}
	}
}
