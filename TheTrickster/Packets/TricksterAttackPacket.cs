using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Classes.Errors;
using ModLibsCore.Services.Network.SimplePacket;
using ModLibsCore.Libraries.Debug;
using TheTrickster.NPCs;


namespace TheTrickster.Packets {
	class TricksterAttackPacket : SimplePacketPayload {
		public static void BroadcastToClients( int npcWho, Vector2 attackCenter ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModLibsException("Not server");
			}

			var payload = new TricksterAttackPacket( npcWho, attackCenter );
			SimplePacket.SendToClient( payload , - 1, -1 );
		}



		////////////////

		public int NpcWho;
		public Vector2 AttackCenter;



		////////////////

		private TricksterAttackPacket() { }

		private TricksterAttackPacket( int npcWho, Vector2 attackCenter ) {
			this.NpcWho = npcWho;
			this.AttackCenter = attackCenter;
		}


		////////////////

		public override void ReceiveOnClient() {
			NPC npc = Main.npc[ this.NpcWho ];
			if( npc?.active != true || npc.type != ModContent.NPCType<TricksterNPC>() ) {
				LogLibraries.AlertOnce( "Invalid Trickster NPC ("+Main.npc[this.NpcWho]?.ToString()+")" );
				return;
			}

			var mynpc = npc.modNPC as TricksterNPC;
			if( mynpc == null ) {
				LogLibraries.WarnOnce( "Trickster is not a Trickster... (is " + npc.FullName + ")?" );
				return;
			}

			mynpc.LaunchAttack( this.AttackCenter, false, true );
		}

		public override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
