using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Buffs;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void LaunchAttack() {
			var config = TheTricksterConfig.Instance;
			int atkRad = config.Get<int>( nameof(TheTricksterConfig.AttackRadius) );
			int radiusSqr = atkRad * atkRad;
			int invulnBuffType = ModContent.BuffType<DegreelessnessBuff>();

			this.HasAttacked = true;

			this.CreateLaunchedAttackFX( atkRad );

			for( int i=0; i<Main.npc.Length; i++ ) {
				NPC otherNpc = Main.npc[i];
				if( otherNpc?.active != true || otherNpc.friendly || otherNpc.immortal ) {
					continue;
				}
				if( otherNpc.whoAmI == this.npc.whoAmI || otherNpc.type == this.npc.type ) {
					continue;
				}
				if( Vector2.DistanceSquared(otherNpc.Center, this.npc.Center) >= radiusSqr ) {
					continue;
				}
				var mynpc = otherNpc.GetGlobalNPC<TheTricksterGlobalNPC>();
				if( mynpc.TricksterBatDurationTicks > 0 ) {
					continue;
				}

				int invulDur = config.Get<int>( nameof(TheTricksterConfig.InvulnTickDuration) );

				// Let's not make bats *that* evil!
				if( npc.aiStyle == 14 ) {
					invulDur = (invulDur * 3) / 4;
				}

				otherNpc.AddBuff( invulnBuffType, invulDur );
			}
		}
	}
}
