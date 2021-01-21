using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using TheTrickster.NPCs;


namespace TheTrickster.Protocols {
	class TricksterStealProtocol : PacketProtocolBroadcast {
		public static void Broadcast( int npcWho, int itemWho ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModHelpersException("Not server");
			}

			var protocol = new TricksterStealProtocol( npcWho, itemWho );
			protocol.SendToClient( -1, -1 );
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

		protected override void ReceiveOnClient() {
			NPC npc = Main.npc[ this.NpcWho ];
			if( !npc.active ) {
				LogHelpers.Warn( "Nonexistent thief (" + this.NpcWho + ")." );
				return;
			}
			if( npc.type != ModContent.NPCType<TricksterNPC>() ) {
				LogHelpers.AlertOnce( "Mismatched NPC type." );
				return;
			}

			Item item = Main.item[ this.ItemWho ];
			if( !item.active ) {
				LogHelpers.Warn( "Nonexistent item stolen ("+this.ItemWho+")." );
				return;
			}

			var myitem = item.GetGlobalItem<TheTricksterGlobalItem>();
			myitem.IsStolenBy = this.NpcWho;
		}


		protected override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}
	}
}
