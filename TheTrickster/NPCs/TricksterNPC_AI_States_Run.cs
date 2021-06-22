using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private TricksterDecision RunAI_States() {
			switch( this.State ) {
			case TricksterState.Mock:
				return this.RunAI_States_Mock();
			case TricksterState.Lurk:
				return this.RunAI_States_Lurk();
			case TricksterState.AttackChargeup:
				return this.RunAI_States_AttackChargeup();
			case TricksterState.Cooldown:
				return this.RunAI_States_Cooldown();
			default:
				return TricksterDecision.None;
			}
		}


		////////////////

		private TricksterDecision RunAI_States_Mock() {
			float helloRange = 16 * 16;
			float helloRangeSqr = helloRange * helloRange;
			bool saidHello = false;

			int plrs = Main.player.Length;
			for( int i=0; i<plrs; i++ ) {
				Player plr = Main.player[i];
				if( plr?.active != true || plr.dead ) {
					continue;
				}

				if( (plr.Center - this.npc.Center).LengthSquared() >= helloRangeSqr ) {
					continue;
				}

				saidHello = true;
				break;
			}

			if( saidHello ) {
				this.EncounterFormal( true );

				return TricksterDecision.FleeNoBombs;
				//this.FleeAction( false );
			} else {
				return TricksterDecision.None;
			}
		}


		////////////////

		private TricksterDecision RunAI_States_Lurk() {
			var config = TheTricksterConfig.Instance;
			int lurkStealRange = config.Get<int>( nameof(TheTricksterConfig.LurkStealRange) );
			float stealRangeSqr = lurkStealRange * lurkStealRange;

			bool hasStolen = false;

			int plrs = Main.player.Length;
			for( int i=0; i<plrs; i++ ) {
				Player plr = Main.player[i];
				if( plr?.active != true || plr.dead ) {
					continue;
				}

				if( (plr.Center - this.npc.Center).LengthSquared() >= stealRangeSqr ) {
					continue;
				}

				hasStolen = this.StealFromPlayer( plr );
				break;
			}

			if( hasStolen ) {
				int minDodgeRad = config.Get<int>( nameof( config.MinDodgeRadius ) );
				int maxDodgeRad = config.Get<int>( nameof( config.MaxDodgeRadius ) );
				maxDodgeRad = ( ( maxDodgeRad - minDodgeRad ) / 3 ) + minDodgeRad;

				this.DodgeAction( minDodgeRad, maxDodgeRad, true );

				this.EncounterFormal( true );

				this.SetState( TricksterState.PreAttack );
			}
			
			return TricksterDecision.None;
		}


		////////////////

		 private int AttackChargingSideEffectCooldown = 0;

		public TricksterDecision RunAI_States_AttackChargeup() {
			if( this.AttackChargingSideEffectCooldown-- <= 0 ) {
				this.AttackChargingSideEffectCooldown = 10;

				return this.RunAI_States_Attack_DeflectProjetiles();
			}

			return TricksterDecision.None;
		}

		private TricksterDecision RunAI_States_Attack_DeflectProjetiles() {
			var config = TheTricksterConfig.Instance;
			int atkRad = config.Get<int>( nameof(config.AttackRadius) );
			float attackRangeSqr = atkRad * atkRad;
			int maxProjs = Main.projectile.Length;
			//int maxPlrs = Main.player.Length;

			for( int i = 0; i < maxProjs; i++ ) {
				Projectile proj = Main.projectile[i];
				if( proj?.active != true ) {
					continue;
				}

				if( (proj.Center - this.npc.Center).LengthSquared() < attackRangeSqr ) {
					proj.velocity *= 0.8f;
				}
			}

			/*for( int i=0; i<maxPlrs; i++ ) {
				Player plr = Main.player[i];
				if( plr?.active != true ) {
					continue;
				}

				if( (plr.Center - this.npc.Center).LengthSquared() < attackRangeSqr ) {
					plr.velocity *= 0.9f;
				}
			}*/

			return TricksterDecision.None;
		}


		////////////////

		private TricksterDecision RunAI_States_Cooldown() {
			var dir = new Vector2( Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() - 0.5f );
			dir.Normalize();

			float arcLength = (16f * 3f) + (Main.rand.NextFloat() * 16f * 4f);
			Vector2 fxPos = npc.Center + (dir * arcLength);

			FX.TricksterChargeArc( fxPos, dir, arcLength, 0.01f, 0.06f );

			return TricksterDecision.None;
		}
	}
}
