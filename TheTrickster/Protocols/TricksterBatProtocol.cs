using System;
using Terraria;
using Terraria.ID;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster.Protocols {
	class TricksterBatProtocol : PacketProtocolBroadcast {
		public static void Broadcast( int npcWho ) {
			if( Main.netMode != 2 ) {
				throw new ModHelpersException("Not server");
			}

			var protocol = new TricksterBatProtocol( npcWho );
			protocol.SendToClient( -1, -1 );
		}



		////////////////

		public int NpcWho;



		////////////////

		private TricksterBatProtocol() { }

		private TricksterBatProtocol( int npcWho ) {
			this.NpcWho = npcWho;
		}


		////////////////

		protected override void ReceiveOnClient() {
			NPC npc = Main.npc[ this.NpcWho ];
			if( !npc.active ) {
				return;
			}
			if( npc.type != NPCID.CaveBat ) {
				LogHelpers.AlertOnce( "Mismatched NPC type." );
				return;
			}

			var mynpc = npc.GetGlobalNPC<TheTricksterGlobalNPC>();
			mynpc.IsTricksterBat = true;
		}

		protected override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
