using System;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using TheTrickster.NPCs;


namespace TheTrickster.Protocols {
	class TricksterStateProtocol : PacketProtocolBroadcast {
		public static void Broadcast( int npcWho, TricksterState state ) {
			if( Main.netMode != 2 ) {
				throw new ModHelpersException("Not server");
			}

			var protocol = new TricksterStateProtocol( npcWho, (int)state );
			protocol.SendToClient( -1, -1 );
		}



		////////////////

		public int NpcWho;
		public int State;



		////////////////

		private TricksterStateProtocol() { }

		private TricksterStateProtocol( int npcWho, int state ) {
			this.NpcWho = npcWho;
			this.State = state;
		}


		////////////////

		protected override void ReceiveOnClient() {
			NPC npc = Main.npc[ this.NpcWho ];
			if( !npc.active ) {
				return;
			}
			if( npc.type == ModContent.NPCType<TricksterNPC>() ) {
				LogHelpers.AlertOnce( "Mismatched NPC type." );
				return;
			}

			var mynpc = npc.modNPC as TricksterNPC;
			if( mynpc == null ) {
				LogHelpers.WarnOnce( "Trickster is not a Trickster...?" );
				return;
			}

			mynpc.SetState( (TricksterState)this.State );
		}

		protected override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
