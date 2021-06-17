using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.Fx;
using ModLibsCore.Libraries.TModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void FleeAction( bool dropBombs ) {
			if( dropBombs ) {
				this.DropBombs();
			}

			ParticleFxLibraries.MakeTeleportFx( this.npc.position, 72, this.npc.width, this.npc.height );

			Main.npc[this.npc.whoAmI] = new NPC();
			this.npc.active = false;

			if( Main.netMode == NetmodeID.Server ) {	// is this needed?
				NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, this.npc.whoAmI );
			}
		}


		public void DropBombs() {
			if( Main.netMode == NetmodeID.MultiplayerClient ) {
				return;
			}

			UnifiedRandom rand = TmlLibraries.SafelyGetRand();
			Vector2 pos = this.npc.position;

			for( int i = 0; i < 3; i++ ) {
				float dirX = 6f * ( rand.NextFloat() - 0.5f );

				int projIdx = Projectile.NewProjectile(
					X: pos.X,
					Y: pos.Y,
					SpeedX: dirX,
					SpeedY: -2f,
					Type: ProjectileID.HappyBomb,
					Damage: 0,
					KnockBack: 0f,
					Owner: Main.myPlayer
				);
				Projectile proj = Main.projectile[ projIdx ];
				proj.timeLeft = 150;

				if( Main.netMode == NetmodeID.Server ) {
					NetMessage.SendData( MessageID.SyncProjectile, -1, -1, null, projIdx );
				}
			}
		}
	}
}
