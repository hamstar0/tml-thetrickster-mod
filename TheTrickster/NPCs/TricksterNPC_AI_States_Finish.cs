using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private void FinishAIState() {
			switch( this.State ) {
			case TricksterState.Idle:
				this.FinishAIState_Idle();
				break;
			case TricksterState.Lurk:
				this.FinishAIState_Lurk();
				break;
			case TricksterState.PreAttack:
				this.FinishAIState_PreAttack();
				break;
			case TricksterState.AttackChargeup:
				this.FinishAIState_Attack();
				break;
			case TricksterState.Cooldown:
				this.FinishAIState_Cooldown();
				break;
			default:
				break;
			}
		}


		////////////////

		private void FinishAIState_Idle() {
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

			this.DodgeAction( minDodgeRad, maxDodgeRad, true );

			this.SetState( TricksterState.PreAttack );
		}


		private void FinishAIState_Lurk() {
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

			this.EncounterFormal( false );

			this.SetState( TricksterState.PreAttack );
		}


		private void FinishAIState_PreAttack() {
			this.FinishAIState_Idle();
		}


		private void FinishAIState_Attack() {
			this.LaunchAttack( this.npc.Center, true, false );

			this.SetState( TricksterState.Cooldown );
		}


		private void FinishAIState_Cooldown() {
			var config = TheTricksterConfig.Instance;
			int minDodgeRad = config.Get<int>( nameof(config.MinDodgeRadius) );
			int maxDodgeRad = config.Get<int>( nameof(config.MaxDodgeRadius) );

			this.DodgeAction( minDodgeRad, maxDodgeRad, true );

			this.SetState( TricksterState.PreAttack );
		}
	}
}
