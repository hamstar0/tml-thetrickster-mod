using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace TheTrickster {
	class TheTricksterWorld : ModWorld {
		public int TricksterDefeats { get; private set; } = 0;



		////////////////

		public override void Load( TagCompound tag ) {
			if( tag.ContainsKey("trickster_defeats") ) {
				this.TricksterDefeats = tag.GetInt( "trickster_defeats" );
			}
		}

		public override TagCompound Save() {
			return new TagCompound { { "trickster_defeats", this.TricksterDefeats } };
		}


		////////////////

		public override void NetReceive( BinaryReader reader ) {
			if( reader == null ) { return; }
			try {
				this.TricksterDefeats = reader.ReadInt32();
			} catch { }
		}

		public override void NetSend( BinaryWriter writer ) {
			if( writer == null ) { return; }
			try {
				writer.Write( (int)this.TricksterDefeats );
			} catch { }
		}


		////////////////

		public void AddTricksterDefeat() {
			this.TricksterDefeats += 1;
		}
	}
}
