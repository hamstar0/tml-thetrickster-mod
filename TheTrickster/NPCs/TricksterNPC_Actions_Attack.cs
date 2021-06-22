using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Services.Timers;
using ModLibsUtilityContent.Buffs;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void LaunchAttack( Vector2 attackCenter, bool syncIfServer, bool forceIfClient ) {
			if( !forceIfClient && Main.netMode == NetmodeID.MultiplayerClient ) {
				return;
			}

			var config = TheTricksterConfig.Instance;
			int atkRad = config.Get<int>( nameof(TheTricksterConfig.AttackRadius) );
			int radiusSqr = atkRad * atkRad;

			this.HasAttacked = true;

			this.CreateLaunchedAttackFX( attackCenter, atkRad );

			for( int i=0; i<Main.npc.Length; i++ ) {
				NPC otherNpc = Main.npc[i];
				if( otherNpc?.active != true || otherNpc.friendly || otherNpc.immortal ) {
					continue;
				}
				if( otherNpc.whoAmI == this.npc.whoAmI || otherNpc.type == this.npc.type ) {
					continue;
				}
				if( Vector2.DistanceSquared(otherNpc.Center, attackCenter ) >= radiusSqr ) {
					continue;
				}

				this.ApplyAttackEffectToNpc( otherNpc );

				//

				if( config.DebugModeInfo ) {
					Vector2 targPos = otherNpc.Center;

					Timers.RunUntil( () => {
						Dust.QuickDust( targPos, Color.Green );
						return true;
					}, 60 * 5, true );
				}
			}

			if( syncIfServer && Main.netMode == NetmodeID.Server ) {
				TricksterAttackProtocol.BroadcastToClients( this.npc.whoAmI, attackCenter );
			}

			//

			if( config.DebugModeInfo ) {
				Timers.RunUntil( () => {
					Dust.QuickDust( attackCenter, Color.Red );
					return true;
				}, 60 * 5, true );
			}
		}


		public bool ApplyAttackEffectToNpc( NPC otherNpc ) {
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
