﻿using System;
using Terraria;
using Terraria.ID;
using ModLibsCore.Classes.Errors;
using ModLibsCore.Services.Network.SimplePacket;
using ModLibsCore.Libraries.Debug;


namespace TheTrickster.Protocols {
	[Serializable]
	class TricksterBatProtocol : SimplePacketPayload {
		public static void BroadcastToClients( int npcWho ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModLibsException("Not server");
			}

			var payload = new TricksterBatProtocol( npcWho );
			SimplePacket.SendToClient( payload , - 1, -1 );
		}



		////////////////

		public int NpcWho;



		////////////////

		private TricksterBatProtocol() { }

		private TricksterBatProtocol( int npcWho ) {
			this.NpcWho = npcWho;
		}


		////////////////

		public override void ReceiveOnClient() {
			NPC npc = Main.npc[ this.NpcWho ];
			if( !npc.active ) {
				return;
			}
			if( npc.type != NPCID.CaveBat ) {
				LogLibraries.AlertOnce( "Mismatched NPC type." );
				return;
			}

			var config = TheTricksterConfig.Instance;
			var mynpc = npc.GetGlobalNPC<TheTricksterGlobalNPC>();
			mynpc.TricksterBatDurationTicks = config.Get<int>( nameof(config.TricksterDefensiveBatTickDuration) );
		}

		public override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
