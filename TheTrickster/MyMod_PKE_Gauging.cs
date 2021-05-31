using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using TheTrickster.NPCs;


namespace TheTrickster {
	public partial class TheTricksterMod : Mod {
		public static float GaugeTricksterPresence( Vector2 worldPos, out float fluctuationRate ) {
			var config = TheTricksterConfig.Instance;
			int maxTileRange = config.Get<int>( nameof( config.TricksterPKEDetectionTileRangeMax ) );
			if( maxTileRange <= 0 ) {
				fluctuationRate = 0f;
				return 0f;
			}

			int tileX = (int)worldPos.X / 16;
			int tileY = (int)worldPos.Y / 16;
			if( !TricksterNPC.CanSpawn(tileX, tileY, Main.LocalPlayer) ) {
				fluctuationRate = 0f;
				return 0f;
			}

			NPC nearest = TheTricksterMod.FindNearestTrickster( worldPos, maxTileRange );

			return TheTricksterMod.GaugeTrickstersNear( worldPos, maxTileRange, nearest, out fluctuationRate );
		}


		public static float GaugeTrickstersNear(
					Vector2 worldPos,
					int maxTileRange,
					NPC nearestTricksterNPC,
					out float fluctuationRate ) {
			float maxWldRange = maxTileRange * 16f;
			float nearestNpcDist = maxWldRange + 1f;
			
			if( nearestTricksterNPC != null ) {
				nearestNpcDist = (nearestTricksterNPC.Center - worldPos).Length();
			}

			float nearestPerc = (float)nearestNpcDist / (float)maxWldRange;
			float gaugedPerc = Math.Max( 1f - nearestPerc, 0f );

			if( nearestTricksterNPC != null ) {
				var mynpc = (TricksterNPC)nearestTricksterNPC.modNPC;

				// Apply 'is alerted' fluctuations
				if( mynpc.IsAlerted ) {
					fluctuationRate = 1f;
				}
				// Apply 'is lurking nearby' fluctuations
				else {
					fluctuationRate = gaugedPerc;
				}
			}
			// Apply 'is able to spawn' fluctuations
			else {
				if( TricksterNPC.IsNearbyOtherTricksterDefeats( (int)(worldPos.X/16f), (int)(worldPos.Y/16) ) ) {
					gaugedPerc = 0f;
					fluctuationRate = 0f;
				} else {
					fluctuationRate = 10f / 60f;
				}
			}

			return gaugedPerc;
		}


		////////////////

		public static NPC FindNearestTrickster( Vector2 worldPos, int maxTileRange ) {
			NPC nearestNpc = null;

			int tricksterType = ModContent.NPCType<TricksterNPC>();

			int maxWldRange = maxTileRange * 16;
			int maxWldRangeSqr = maxWldRange * maxWldRange;
			int nearestWldDistSqr = Int32.MaxValue;

			int maxNpcs = Main.npc.Length;
			for( int i = 0; i < maxNpcs; i++ ) {
				NPC npc = Main.npc[i];
				if( npc?.active != true || npc.type != tricksterType ) {
					continue;
				}

				int distSqr = (int)( worldPos - npc.Center ).LengthSquared();
				if( distSqr >= maxWldRangeSqr ) {
					continue;
				}

				if( distSqr < nearestWldDistSqr ) {
					nearestWldDistSqr = distSqr;
					nearestNpc = npc;
				}
			}

			return nearestNpc;
		}
	}
}