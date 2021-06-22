using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Classes.Errors;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Services.Network.SimplePacket;
using TheTrickster.NPCs;


namespace TheTrickster.Protocols {
	class TricksterStealProtocol : SimplePacketPayload {
		public static void BroadcastToClients( int npcWho, int itemWho ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModLibsException("Not server");
			}

			var packet = new TricksterStealProtocol( npcWho, itemWho );
			SimplePacket.SendToClient( packet, -1, -1 );
		}



		////////////////

		public int NpcWho;
		public int ItemWho;



		////////////////

		private TricksterStealProtocol() { }

		private TricksterStealProtocol( int npcWho, int itemWho ) {
			this.NpcWho = npcWho;
			this.ItemWho = itemWho;
		}


		////////////////

		public override void ReceiveOnClient() {
			NPC npc = Main.npc[ this.NpcWho ];
			if( npc?.active != true || npc.type != ModContent.NPCType<TricksterNPC>() ) {
				LogLibraries.Alert( "Invalid Trickster NPC ("+Main.npc[this.NpcWho]?.ToString()+")" );
				return;
			}

			Item item = Main.item[ this.ItemWho ];
			if( !item.active ) {
				LogLibraries.Warn( "Nonexistent item stolen ("+this.ItemWho+")." );
				return;
			}

			var myitem = item.GetGlobalItem<TheTricksterGlobalItem>();
			myitem.IsStolenBy = this.NpcWho;
		}


		public override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
