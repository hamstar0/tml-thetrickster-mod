﻿using System;
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
			}
		}


		////////////////
		
		private Color GetLurkDrawColor( Color drawColor ) {
			float scale = Main.rand.NextFloat();
			scale = (float)Math.Pow( scale, 30 );
			scale = 0.25f + (0.55f * scale);

			return drawColor * scale;
		}


		////////////////

		public void EncounterFX() {
			Vector2 scrMid = Main.screenPosition;
			scrMid.X += Main.screenWidth / 2;
			scrMid.Y += Main.screenHeight / 2;
			float distSqr = Vector2.DistanceSquared( scrMid, this.npc.Center );

			if( distSqr < 409600 ) {    //640
				Vector2 diff = this.npc.Center - scrMid;
				Vector2 pos = scrMid + ( diff * 0.5f );

				int soundSlot = this.mod.GetSoundSlot( SoundType.Custom, "Sounds/Custom/TricksterLaugh" );
				Main.PlaySound(
					type: (int)SoundType.Custom,
					x: (int)pos.X,
					y: (int)pos.Y,
					Style: soundSlot,
					volumeScale: 0.8f
				);
			}
		}
	}
}
