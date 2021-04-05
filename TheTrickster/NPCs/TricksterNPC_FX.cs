using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private void RunFX( bool isNewlyAlerted ) {
			/*if( isNewlyAlerted ) {
				this.EncounterFX();
			}*/

			if( this.State == TricksterState.AttackChargeup ) {
				this.RunFX_Attack( isNewlyAlerted );
			} else {
				this.RunFX_UnAttack( isNewlyAlerted );
			}
		}


		////////////////
		
		private Color GetLurkDrawColor( Color drawColor ) {
			float scale = Main.rand.NextFloat();
			scale = (float)Math.Pow( scale, 30 );
			scale = 0.25f + (0.55f * scale);

			return drawColor * scale;
		}
	}
}
