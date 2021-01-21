using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Buffs;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Fx;
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
				ParticleFxHelpers.MakeDustCloud( this.npc.Center, 3, 0.3f, 2f );
			}
		}

		private void DeployDefenseBat() {
			int npcWho = NPC.NewNPC( (int)this.npc.position.X, (int)this.npc.position.Y, NPCID.CaveBat, 0, 0f, 0f, 0f, 0f, this.npc.target );
			NPC npc = Main.npc[npcWho];
			if( !npc.active ) {
				return;
			}

			npc.scale = 0.65f;
			npc.life = 1;
			npc.lifeMax = 1;
			npc.defense = 999999;
			npc.damage = 1;
			//npc.color = XNAColorHelpers.Mul( npc.color, Color.Red );
			npc.value = 0;
			npc.SpawnedFromStatue = true;	// no loot abuse?
			npc.velocity = new Vector2(
				Main.rand.NextFloat() - 0.5f,
				Main.rand.NextFloat() - 0.5f
			) * 8f;

			var config = TheTricksterConfig.Instance;
			var mynpc = npc.GetGlobalNPC<TheTricksterGlobalNPC>();
			mynpc.TricksterBatDurationTicks = config.Get<int>( nameof(config.TricksterDefensiveBatTickDuration) );

			if( Main.netMode == NetmodeID.Server ) {
				NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, npc.whoAmI );
				TricksterBatProtocol.Broadcast( npcWho );
			}

			npc.AddBuff( ModContent.BuffType<DegreelessnessBuff>(), 60 );
		}
	}
}
