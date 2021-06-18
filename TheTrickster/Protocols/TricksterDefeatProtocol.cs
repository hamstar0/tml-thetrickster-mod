using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Classes.Errors;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Services.Network.SimplePacket;


namespace TheTrickster.Protocols {
	class TricksterDefeatProtocol : SimplePacketPayload {
		public static void BroadcastToClients( int tileX, int tileY ) {
			if( Main.netMode != NetmodeID.Server ) {
				throw new ModLibsException("Not server");
			}

			var packet = new TricksterDefeatProtocol( tileX, tileY );
			SimplePacket.SendToClient( packet , -1, -1 );
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

		public override void ReceiveOnServer( int fromWho ) {
			throw new NotImplementedException();
		}

		public override void ReceiveOnClient() {
			var myworld = ModContent.GetInstance<TheTricksterWorld>();

			myworld.AddTricksterDefeat( this.TileX, this.TileY );
		}
	}
}
