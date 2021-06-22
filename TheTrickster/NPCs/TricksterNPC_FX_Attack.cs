using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.TModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public static void AnimateAttackBurstFX( Vector2 position, float radius, int particles ) {
			UnifiedRandom rand = TmlLibraries.SafelyGetRand();

			for( int i = 0; i < particles; i++ ) {
				Vector2 dir = new Vector2( rand.NextFloat() - 0.5f, rand.NextFloat() - 0.5f );
				dir.Normalize();
				Vector2 dustPos = position + (dir * rand.NextFloat() * radius);

				FX.TricksterAttackSpark( dustPos );
			}
		}

		public static void AnimateAttackChargeAreaFX( Vector2 position, float percentCharged, int particles ) {
			UnifiedRandom rand = TmlLibraries.SafelyGetRand();
			float radius = 80f;
			bool willDrawLightning = (rand.NextFloat() * 3f) < (percentCharged * percentCharged);

			particles = (particles / 2) + (int)(percentCharged * (float)(particles / 2));

			for( int i = 0; i < particles; i++ ) {
				if( rand.NextFloat() >= percentCharged ) { continue; }

				var dir = new Vector2( rand.NextFloat() - 0.5f, rand.NextFloat() - 0.5f );
				dir.Normalize();

				float arcLength = 8f + (rand.NextFloat() * (radius - 8f));
				arcLength *= 0.75f + percentCharged;

				Vector2 dustPos = position + (dir * arcLength);

				Vector2 pullVelocity = -dir * ( arcLength / 8f );

				FX.TricksterChargeSpark( dustPos, pullVelocity );

				if( willDrawLightning ) {
					willDrawLightning = false;

					FX.TricksterChargeArc( dustPos, dir, arcLength * 4f );
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

				if( this.AttackChargeSoundInstance != null ) {
					this.AttackChargeSoundInstance.Volume *= 0.25f + ( percent * 0.75f );
				}
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

		public void CreateLaunchedAttackFX( Vector2 attackCenter, int radius ) {
			Main.PlaySound( SoundID.Item113, attackCenter );

			Task.Run( () => TricksterNPC.AnimateAttackBurstFX( attackCenter, radius, radius / 3 ) );
			Task.Run( () => TricksterNPC.AnimateAttackBurstFX( attackCenter, radius, radius / 3 ) );
			Task.Run( () => TricksterNPC.AnimateAttackBurstFX( attackCenter, radius, radius / 3 ) );
			Task.Run( () => {
				TricksterNPC.AnimateAttackBurstFX( attackCenter, 64, 64 );
				TricksterNPC.AnimateAttackBurstFX( attackCenter, 32, 32 );
			} );
		}
	}
}
