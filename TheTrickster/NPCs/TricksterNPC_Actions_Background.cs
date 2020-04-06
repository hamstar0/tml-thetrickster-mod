using System;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private int AttackChargingSideEffectCooldown = 0;


		public void AttackChargingSideEffects() {
			if( this.AttackChargingSideEffectCooldown-- > 0 ) {
				return;
			}
			this.AttackChargingSideEffectCooldown = 10;

			var config = TheTricksterConfig.Instance;
			float attackRangeSqr = config.AttackRadius * config.AttackRadius;
			int maxProjs = Main.projectile.Length;
			//int maxPlrs = Main.player.Length;

			for( int i = 0; i < maxProjs; i++ ) {
				Projectile proj = Main.projectile[i];
				if( proj?.active != true ) {
					continue;
				}

				DebugHelpers.Print( "proj_range_" + proj.whoAmI, "lenSqr: " + (int)( proj.Center - this.npc.Center ).LengthSquared() + " vs " + (int)attackRangeSqr );
				if( ( proj.Center - this.npc.Center ).LengthSquared() < attackRangeSqr ) {
					Main.NewText( proj.Name + " - was " + proj.velocity.ToShortString() + ", is: " + ( proj.velocity * 0.8f ).ToShortString() );
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
