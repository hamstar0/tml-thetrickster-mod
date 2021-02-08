using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private void RunAI_States_Finish() {
			switch( this.State ) {
			case TricksterState.Idle:
				this.RunAI_States_Finish_Idle();
				break;
			case TricksterState.Lurk:
				this.RunAI_States_Finish_Lurk();
				break;
			case TricksterState.PreAttack:
				this.RunAI_States_Finish_PreAttack();
				break;
			case TricksterState.Attack:
				this.RunAI_States_Finish_Attack();
				break;
			case TricksterState.Cooldown:
				this.RunAI_States_Finish_Cooldown();
				break;
			default:
				break;
			}
		}


		////////////////

		private void RunAI_States_Finish_Idle() {
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


		private void RunAI_States_Finish_Lurk() {
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


		private void RunAI_States_Finish_PreAttack() {
			this.RunAI_States_Finish_Idle();
		}


		private void RunAI_States_Finish_Attack() {
			this.LaunchAttack();

			this.SetState( TricksterState.Cooldown );
		}


		private void RunAI_States_Finish_Cooldown() {
			var config = TheTricksterConfig.Instance;
			int minDodgeRad = config.Get<int>( nameof(config.MinDodgeRadius) );
			int maxDodgeRad = config.Get<int>( nameof(config.MaxDodgeRadius) );

			this.DodgeAction( minDodgeRad, maxDodgeRad );

			this.SetState( TricksterState.PreAttack );
		}
	}
}
