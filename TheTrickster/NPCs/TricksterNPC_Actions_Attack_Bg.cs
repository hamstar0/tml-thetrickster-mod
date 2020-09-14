using System;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private int AttackChargingSideEffectCooldown = 0;



		////////////////

		public void AttackChargingSideEffects() {
			if( this.AttackChargingSideEffectCooldown-- > 0 ) {
				return;
			}
			this.AttackChargingSideEffectCooldown = 10;

			var config = TheTricksterConfig.Instance;
			int atkRad = config.Get<int>( nameof(TheTricksterConfig.AttackRadius) );
			float attackRangeSqr = atkRad * atkRad;
			int maxProjs = Main.projectile.Length;
			//int maxPlrs = Main.player.Length;

			for( int i = 0; i < maxProjs; i++ ) {
				Projectile proj = Main.projectile[i];
				if( proj?.active != true ) {
					continue;
				}

				if( ( proj.Center - this.npc.Center ).LengthSquared() < attackRangeSqr ) {
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
		}
	}
}
