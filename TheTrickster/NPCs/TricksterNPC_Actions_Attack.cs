using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsUtilityContent.Buffs;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void LaunchAttack() {
			var config = TheTricksterConfig.Instance;
			int atkRad = config.Get<int>( nameof(TheTricksterConfig.AttackRadius) );
			int radiusSqr = atkRad * atkRad;

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

				this.ApplyAttackEffectToNpc( otherNpc );
			}
		}


		public bool ApplyAttackEffectToNpc( NPC otherNpc ) {
			if( Main.netMode == NetmodeID.MultiplayerClient ) {
				return false;
			}

			var config = TheTricksterConfig.Instance;
			int invulnBuffType = ModContent.BuffType<DegreelessnessBuff>();

			var mynpc = otherNpc.GetGlobalNPC<TheTricksterGlobalNPC>();
			if( mynpc.TricksterBatDurationTicks > 0 ) {
				return false;
			}

			int invulDur = config.Get<int>( nameof(TheTricksterConfig.InvulnTickDuration) );

			// Let's not make bats *that* evil!
			if( npc.aiStyle == 14 ) {
				float scale = config.Get<float>( nameof( TheTricksterConfig.InvulnTickDurationScaleForBats ) );
				invulDur = (int)((float)invulDur * scale );
			}

			otherNpc.AddBuff( invulnBuffType, invulDur );

			return true;
		}
	}
}
