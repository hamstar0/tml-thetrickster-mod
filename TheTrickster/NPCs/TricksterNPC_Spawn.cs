using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.World;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public static bool IsWithinSpawnRange( int tileX, int tileY ) {
			// Underground and underworld only
			return tileY >= WorldHelpers.RockLayerTopTileY;
		}



		////////////////

		public override float SpawnChance( NPCSpawnInfo spawnInfo ) {
			int tileX = spawnInfo.spawnTileX;
			int tileY = spawnInfo.spawnTileY;
			if( TricksterNPC.IsWithinSpawnRange(tileX, tileY) ) {
				return 0f;
			}

			// Should have nearby NPCs
			if( spawnInfo.player.activeNPCs < 3f ) {
				return 0f;
			}

			// "Normal" biomes only
			/*spawnInfo.player.ZoneJungle ||*/
			if( spawnInfo.player.ZoneHoly || spawnInfo.player.ZoneCorrupt || spawnInfo.player.ZoneCrimson ) {
				return 0f;
			}

			// No sandstorms
			if( spawnInfo.player.ZoneSandstorm ) {
				return 0f;
			}

			// If on surface, only spawn where there's background walls
			if( tileY < WorldHelpers.SurfaceLayerBottomTileY ) {
				if( (Main.tile[tileX, tileY]?.wall ?? 0) == 0 ) {
					return 0f;
				}
			}

			// Only one at a time
			if( Main.npc.Any(n => n?.active == true && n.netID == ModContent.NPCType<TricksterNPC>()) ) {
				return 0f;
			}

			if( TricksterNPC.IsNearbyOtherTricksterDefeats(tileX, tileY) ) {
				return 0f;
			}

			return TheTricksterConfig.Instance.Get<float>( nameof(TheTricksterConfig.SpawnChance) );
		}


		////

		public override int SpawnNPC( int tileX, int tileY ) {
			int npcWho = base.SpawnNPC( tileX, tileY );
			NPC npc = Main.npc[npcWho];

			int nearPlrWho = npc.FindClosestPlayer();
			npc.target = nearPlrWho;

			var mynpc = npc.modNPC as TricksterNPC;
			mynpc.SetState( TricksterState.Lurk );

			return npcWho;
		}
	}
}
