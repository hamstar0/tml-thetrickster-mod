using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.TModLoader;
using ModLibsGeneral.Libraries.Fx;
using ModLibsGeneral.Libraries.World;
using ModLibsTiles.Classes.Tiles.TilePattern;
using ModLibsGeneral.Libraries.Tiles;

namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void DodgeAction( int minDodgeRadius, int maxDodgeRadius ) {
			UnifiedRandom rand = TmlLibraries.SafelyGetRand();
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

				isOnGround = TileWorldLibraries.DropToGround(
					worldPos: testPos,
					invertGravity: false,
					isGround: (x2, y2) => TilePattern.CommonSolid.Check(x2, y2),
					groundPos: out groundPos
				);
				if( !isOnGround ) { continue; }

				if( Vector2.DistanceSquared(player.Center, groundPos) < minDistSqr ) {
					isOnGround = false;
					continue;
				}

				tileX = (int)groundPos.X / 16;
				tileY = (int)groundPos.Y / 16;
			} while( !isOnGround || !validLanding.CheckArea( new Rectangle(tileX-1, tileY-3, 3, 3) ) );

			// Before
			ParticleFxLibraries.MakeTeleportFx( this.npc.position, 48, this.npc.width, this.npc.height );

			if( Main.netMode != NetmodeID.MultiplayerClient ) {
				this.npc.position = groundPos - new Vector2( 0, this.npc.height + 1 );

				if( Main.netMode == NetmodeID.Server ) {
					NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, this.npc.whoAmI );
				}
			}

			// After
			ParticleFxLibraries.MakeTeleportFx( this.npc.position, 48, this.npc.width, this.npc.height );
		}
	}
}
