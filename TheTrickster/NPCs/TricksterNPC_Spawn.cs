using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.World;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public static bool IsWithinSpawnRange( int tileX, int tileY ) {
			//return tileY >= WorldHelpers.RockLayerTopTileY; // Underground and underworld only
			return true;
		}

		public static bool CanSpawn( int tileX, int tileY, Player player ) {
			if( !TricksterNPC.IsWithinSpawnRange( tileX, tileY ) ) {
				return false;
			}

			// Should have nearby NPCs
			if( player.activeNPCs < 3f ) {
				return false;
			}

			// "Normal" biomes only
			/*spawnInfo.player.ZoneJungle ||*/
			if( player.ZoneHoly || player.ZoneCorrupt || player.ZoneCrimson ) {
				return false;
			}

			// No sandstorms
			if( player.ZoneSandstorm ) {
				return false;
			}

			// If on surface, only spawn where there's background walls
			if( tileY < WorldHelpers.SurfaceLayerBottomTileY ) {
				if( ( Main.tile[tileX, tileY]?.wall ?? 0 ) == 0 ) {
					return false;
				}
			}

			// Only one at a time
			if( Main.npc.Any( n => n?.active == true && n.netID == ModContent.NPCType<TricksterNPC>() ) ) {
				return false;
			}

			if( TricksterNPC.IsNearbyOtherTricksterDefeats( tileX, tileY ) ) {
				return false;
			}

			return true;
		}



		////////////////

		public override float SpawnChance( NPCSpawnInfo spawnInfo ) {
			int tileX = spawnInfo.spawnTileX;
			int tileY = spawnInfo.spawnTileY;
			if( !TricksterNPC.CanSpawn(tileX, tileY, spawnInfo.player) ) {
				return 0f;
			}

			if( tileY < WorldHelpers.SurfaceLayerBottomTileY ) {
				return TheTricksterConfig.Instance.Get<float>( nameof(TheTricksterConfig.SpawnChanceAboveGround) );
			} else {
				return TheTricksterConfig.Instance.Get<float>( nameof(TheTricksterConfig.SpawnChanceUnderGround) );
			}
		}


		////////////////

		public override int SpawnNPC( int tileX, int tileY ) {
			int npcWho = base.SpawnNPC( tileX, tileY );
			NPC myNpc = Main.npc[npcWho];

			int nearPlrWho = myNpc.FindClosestPlayer();
			myNpc.target = nearPlrWho;

			var mynpc = myNpc.modNPC as TricksterNPC;
			TricksterState state = this.DecideOnSpawnState();

			mynpc.SetState( state );

			//if( Main.netMode == NetmodeID.Server ) {	<- Is this handled by SendExtraAI?
			//	TricksterStateProtocol.Broadcast( this.npc.whoAmI, state );
			//}

			return npcWho;
		}
	}
}
