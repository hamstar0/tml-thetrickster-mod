using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public override void SendExtraAI( BinaryWriter writer ) {
			writer.Write( (bool)this.IsAlerted );

			writer.Write( (byte)this.State );
		}


		public override void ReceiveExtraAI( BinaryReader reader ) {
			bool isAlerted = reader.ReadBoolean();
			TricksterState state = (TricksterState)reader.ReadByte();

			this.IsAlerted = isAlerted;

			if( state != this.State ) {
				//this.SetState( state, false, true );	<- State changes handle their own sync, separately and manually
				this.State = state;

				this.HitsDuringCurrentStage = 0;
				this.ElapsedStateTicks = 0;
			}
		}
	}
}
