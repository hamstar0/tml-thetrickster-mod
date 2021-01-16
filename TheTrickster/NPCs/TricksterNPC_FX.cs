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

			if( this.State == TricksterState.Attack ) {
				this.RunFX_Attack( isNewlyAlerted );
			} else {
				this.RunFX_UnAttack( isNewlyAlerted );

				if( this.State == TricksterState.Lurk ) {
					this.RunFX_Lurk( isNewlyAlerted );
				}
			}
		}


		////////////////
		
		private void RunFX_Lurk( bool isNewlyAlerted ) { }


		////////////////

		public void EncounterFX() {
			Vector2 scrMid = Main.screenPosition;
			scrMid.X += Main.screenWidth / 2;
			scrMid.Y += Main.screenHeight / 2;
			float distSqr = Vector2.DistanceSquared( scrMid, this.npc.Center );

			if( distSqr < 409600 ) {
				Vector2 diff = this.npc.Center - scrMid;
				Vector2 pos = scrMid + ( diff * 0.5f );

				int soundSlot = this.mod.GetSoundSlot( SoundType.Custom, "Sounds/Custom/TricksterLaugh" );
				Main.PlaySound( (int)SoundType.Custom, (int)pos.X, (int)pos.Y, soundSlot );
			}
		}
	}
}
