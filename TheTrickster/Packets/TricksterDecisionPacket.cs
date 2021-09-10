using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Classes.Errors;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Services.Network.SimplePacket;
using TheTrickster.NPCs;


namespace TheTrickster.Packets {
	class TricksterDecisionPacket : SimplePacketPayload {
		public static void BroadcastToClients( int npcWho, TricksterDecision decision ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModLibsException("Not server");
			}

			var packet = new TricksterDecisionPacket( npcWho, (int)decision );
			SimplePacket.SendToClient( packet, -1, -1 );
		}



		////////////////

		public int NpcWho;
		public int Decision;



		////////////////

		private TricksterDecisionPacket() { }

		private TricksterDecisionPacket( int npcWho, int decision ) {
			this.NpcWho = npcWho;
			this.Decision = decision;
		}


		////////////////

		public override void ReceiveOnClient() {
			NPC npc = Main.npc[this.NpcWho];
			if( npc?.active != true || npc.type != ModContent.NPCType<TricksterNPC>() ) {
				LogLibraries.AlertOnce( "Invalid Trickster NPC (decision "+this.Decision.ToString()
					+", " + Main.npc[this.NpcWho]?.ToString() + ")" );
				return;
			}

			var mynpc = npc.modNPC as TricksterNPC;
			if( mynpc == null ) {
				LogLibraries.WarnOnce( "Trickster is not a Trickster... (is "+npc.FullName+")?" );
				return;
			}

			mynpc.EnactAIDecision( (TricksterDecision)this.Decision, false );
		}

		public override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
