using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Buffs;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void LaunchAttack() {
			this.HasAttacked = true;

			this.CreateLaunchedAttackFX( TheTricksterConfig.Instance.AttackRadius );

			int radiusSqr = TheTricksterConfig.Instance.AttackRadius * TheTricksterConfig.Instance.AttackRadius;
			int invulnBuffType = ModContent.BuffType<DegreelessnessBuff>();

			for( int i=0; i<Main.npc.Length; i++ ) {
				NPC otherNpc = Main.npc[i];
				if( otherNpc?.active != true || otherNpc.friendly || otherNpc.immortal ) {
					continue;
				}
				if( otherNpc.whoAmI == this.npc.whoAmI || otherNpc.type == this.npc.type ) {
					continue;
				}

				if( Vector2.DistanceSquared(otherNpc.Center, this.npc.Center) < radiusSqr ) {
					var mynpc = otherNpc.GetGlobalNPC<TheTricksterGlobalNPC>();
					if( mynpc.IsTricksterBat ) {
						continue;
					}

					otherNpc.AddBuff( invulnBuffType, TheTricksterConfig.Instance.InvulnTickDuration );
				}
			}
		}
	}
}
