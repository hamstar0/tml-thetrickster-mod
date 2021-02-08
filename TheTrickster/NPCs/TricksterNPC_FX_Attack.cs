using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.TModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public static void AnimateAttackBurstFX( Vector2 position, float radius, int particles ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();

			for( int i = 0; i < particles; i++ ) {
				Vector2 dir = new Vector2( rand.NextFloat() - 0.5f, rand.NextFloat() - 0.5f );
				dir.Normalize();
				Vector2 dustPos = position + (dir * rand.NextFloat() * radius);

				FX.TricksterAttackSpark( dustPos );
			}
		}

		public static void AnimateAttackChargeAreaFX( Vector2 position, float percent, int particles ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			float radius = 80f;
			bool willDrawLightning = (rand.NextFloat() * 3f) < (percent * percent);

			particles = (particles / 2) + (int)(percent * (float)(particles / 2));

			for( int i = 0; i < particles; i++ ) {
				if( rand.NextFloat() >= percent ) { continue; }

				var dir = new Vector2( rand.NextFloat() - 0.5f, rand.NextFloat() - 0.5f );
				dir.Normalize();

				float offset = 8f + ( rand.NextFloat() * ( radius - 8f ) );
				offset *= 0.75f + percent;

				Vector2 dustPos = position + (dir * offset);

				Vector2 pullVelocity = -dir * ( offset / 8f );

				FX.TricksterChargeSpark( dustPos, pullVelocity );

				if( willDrawLightning ) {
					willDrawLightning = false;

					FX.TricksterChargeArc( dustPos, dir, offset );
				}
			}
		}



		////////////////

		private SoundEffectInstance AttackChargeSoundInstance;



		////////////////

		private void RunFX_Attack( bool isNewlyAlerted ) {
			/*if( isNewlyAlerted ) {
				this.EncounterFX();
			}*/

			float percent = (float)this.ElapsedStateTicks / (float)TricksterNPC.GetCurrentStateTickDuration(this.State);

			if( this.AttackChargeSoundInstance == null || this.AttackChargeSoundInstance.State != SoundState.Playing ) {
				/*float distScale = 1f - (Vector2.Distance( Main.LocalPlayer.Center, this.npc.Center ) / 1600f);
				distScale = Math.Max( 0f, distScale );

				if( distScale > 0 ) {
					this.AttackChargeSoundInstance = Main.PlaySound( SoundID.Item93, this.npc.Center );
					this.AttackChargeSoundInstance.Volume = 0.25f + ((percent * 0.75f) * distScale);
				}	//<- volume doesnt work?*/
				this.AttackChargeSoundInstance = Main.PlaySound( SoundID.Item93, this.npc.Center );
				this.AttackChargeSoundInstance.Volume *= 0.25f + ( percent * 0.75f );
			}

			TricksterNPC.AnimateAttackChargeAreaFX( this.npc.Center, percent, 12 );
		}

		private void RunFX_UnAttack( bool isNewlyAlerted ) {
			if( this.AttackChargeSoundInstance != null && this.AttackChargeSoundInstance.State == SoundState.Playing ) {
				this.AttackChargeSoundInstance.Stop();
				this.AttackChargeSoundInstance = null;
			}
		}


		////

		public void CreateLaunchedAttackFX( int radius ) {
			Vector2 pos = this.npc.Center;

			Main.PlaySound( SoundID.Item113, pos );

			Task.Run( () => TricksterNPC.AnimateAttackBurstFX( pos, radius, radius / 3 ) );
			Task.Run( () => TricksterNPC.AnimateAttackBurstFX( pos, radius, radius / 3 ) );
			Task.Run( () => TricksterNPC.AnimateAttackBurstFX( pos, radius, radius / 3 ) );
			Task.Run( () => {
				TricksterNPC.AnimateAttackBurstFX( pos, 64, 64 );
				TricksterNPC.AnimateAttackBurstFX( pos, 32, 32 );
			} );
		}
	}
}
