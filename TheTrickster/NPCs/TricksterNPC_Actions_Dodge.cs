using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Fx;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Helpers.World;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void DodgeAction( int minDodgeRadius, int maxDodgeRadius ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			int minDistSqr = minDodgeRadius * minDodgeRadius;

			int plrWho = this.npc.HasPlayerTarget
					? this.npc.target
					: this.npc.FindClosestPlayer();
			if( plrWho == -1 ) {
				return;
			}

			Player player = Main.player[ plrWho ];
			if( player == null || !player.active ) {
				return;
			}

			var validLanding = new TilePattern( new TilePatternBuilder {
				HasLava = false,
				HasHoney = false,
				CustomCheck = ( x, y ) => {
					Tile tile = Main.tile[x, y];
					return !tile.active() || tile.inActive() || !Main.tileSolid[tile.type];
				}
			} );
			//validLanding = TilePattern.CombineNegative( TilePattern.NonSolid, validLanding );
			
			Vector2 dir, testPos, groundPos;
			bool isOnGround;
			int tileX=0, tileY=0;
			do {
				dir = new Vector2( rand.NextFloat() - 0.5f, rand.NextFloat() - 0.5f );
				dir.Normalize();
				dir *= minDodgeRadius + ( rand.NextFloat() * (maxDodgeRadius - minDodgeRadius ) );

				testPos = player.Center + dir;
				maxDodgeRadius += 1;

				isOnGround = WorldHelpers.DropToGround( testPos, false, TilePattern.CommonSolid, out groundPos );
				if( !isOnGround ) { continue; }

				if( Vector2.DistanceSquared(player.Center, groundPos) < minDistSqr ) {
					isOnGround = false;
					continue;
				}

				tileX = (int)groundPos.X / 16;
				tileY = (int)groundPos.Y / 16;
			} while( !isOnGround || !validLanding.CheckArea( new Rectangle(tileX-1, tileY-3, 3, 3) ) );

			// Before
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 48, this.npc.width, this.npc.height );

			if( Main.netMode != NetmodeID.MultiplayerClient ) {
				this.npc.position = groundPos - new Vector2( 0, this.npc.height + 1 );

				if( Main.netMode == NetmodeID.Server ) {
					NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, this.npc.whoAmI );
				}
			}

			// After
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 48, this.npc.width, this.npc.height );
		}
	}
}
