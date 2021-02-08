using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster.Protocols {
	class TricksterDefeatProtocol : PacketProtocolSendToClient {
		public static void Send( int tileX, int tileY ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModHelpersException("Not server");
			}

			var protocol = new TricksterDefeatProtocol( tileX, tileY );
			protocol.SendToClient( -1, -1 );
		}



		////////////////

		public int TileX;
		public int TileY;



		////////////////

		private TricksterDefeatProtocol() { }

		private TricksterDefeatProtocol( int tileX, int tileY ) {
			this.TileX = tileX;
			this.TileY = tileY;
		}


		////////////////

		protected override void InitializeServerSendData( int toWho ) {
		}

		protected override void Receive() {
			var myworld = ModContent.GetInstance<TheTricksterWorld>();

			myworld.AddTricksterDefeat( this.TileX, this.TileY );
		}
	}
}
