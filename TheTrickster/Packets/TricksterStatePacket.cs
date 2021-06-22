using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Classes.Errors;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Services.Network.SimplePacket;
using TheTrickster.NPCs;


namespace TheTrickster.Packets {
	class TricksterStatePacket : SimplePacketPayload {
		public static void BroadcastToClients( int npcWho, TricksterState state ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModLibsException("Not server");
			}

			var packet = new TricksterStatePacket( npcWho, (int)state );
			SimplePacket.SendToClient( packet, -1, -1 );
		}



		////////////////

		public int NpcWho;
		public int State;



		////////////////

		private TricksterStatePacket() { }

		private TricksterStatePacket( int npcWho, int state ) {
			this.NpcWho = npcWho;
			this.State = state;
		}


		////////////////

		public override void ReceiveOnClient() {
			NPC npc = Main.npc[ this.NpcWho ];
			if( npc?.active != true || npc.type != ModContent.NPCType<TricksterNPC>() ) {
				LogLibraries.Alert( "Invalid Trickster NPC ("+Main.npc[this.NpcWho]?.ToString()+")" );
				return;
			}

			var mynpc = npc.modNPC as TricksterNPC;
			if( mynpc == null ) {
				LogLibraries.WarnOnce( "Trickster is not a Trickster... (is "+npc.FullName+")?" );
				return;
			}

			mynpc.SetState(
				newState: (TricksterState)this.State,
				syncsIfServer: false,
				forcedForClient: true
			);
		}

		public override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
