using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.Fx;
using ModLibsUtilityContent.Buffs;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void DeployDefenseBats() {
			if( Main.netMode != NetmodeID.MultiplayerClient ) {
				int maxBats = Main.rand.Next( 2 );

				for( int i=0; i<maxBats; i++ ) {
					this.DeployDefenseBat();
				}
			}

			if( Main.netMode != NetmodeID.Server ) {
				ParticleFxLibraries.MakeDustCloud( this.npc.Center, 3, 0.3f, 2f );
			}
		}

		private void DeployDefenseBat() {
			int batNpcWho = NPC.NewNPC( (int)this.npc.position.X, (int)this.npc.position.Y, NPCID.CaveBat, 0, 0f, 0f, 0f, 0f, this.npc.target );
			NPC batNpc = Main.npc[batNpcWho];
			if( !batNpc.active ) {
				return;
			}

			batNpc.scale = 0.65f;
			batNpc.life = 1;
			batNpc.lifeMax = 1;
			batNpc.defense = 999999;
			batNpc.damage = 1;
			//npc.color = XNAColorLibraries.Mul( npc.color, Color.Red );
			batNpc.value = 0;
			batNpc.SpawnedFromStatue = true;	// no loot abuse?
			batNpc.velocity = new Vector2(
				Main.rand.NextFloat() - 0.5f,
				Main.rand.NextFloat() - 0.5f
			) * 8f;

			var config = TheTricksterConfig.Instance;
			var mynpc = batNpc.GetGlobalNPC<TheTricksterGlobalNPC>();
			mynpc.TricksterBatDurationTicks = config.Get<int>( nameof(config.TricksterDefensiveBatTickDuration) );

			if( Main.netMode == NetmodeID.Server ) {
				NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, batNpc.whoAmI );
				TricksterBatProtocol.BroadcastToClients( batNpcWho );
			}

			batNpc.AddBuff( ModContent.BuffType<DegreelessnessBuff>(), 60 );
		}
	}
}
