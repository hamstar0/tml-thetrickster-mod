using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.World;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public static bool IsWithinSpawnRange( int tileX, int tileY ) {
			//return tileY >= WorldLibraries.RockLayerTopTileY; // Underground and underworld only
			return true;
		}

		////

		public static bool CanSpawn( int tileX, int tileY, Player player ) {
			var config = TheTricksterConfig.Instance;

			if( !config.DebugModeSpawnsIgnoreRange ) {
				if( !TricksterNPC.IsWithinSpawnRange(tileX, tileY) ) {
					return false;
				}
			}

			if( !config.DebugModeSpawnsIgnoreNpcs ) {
				// Should have nearby NPCs
				if( player.activeNPCs < config.Get<int>( nameof(config.RequiredNearbyNpcsForSpawn) ) ) {
					return false;
				}
			}

			if( !config.DebugModeSpawnsIgnoreLocation ) {
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
				if( tileY < WorldLocationLibraries.SurfaceLayerBottomTileY ) {
					if( ( Main.tile[tileX, tileY]?.wall ?? 0 ) == 0 ) {
						return false;
					}
				}
			}
			
			if( !config.DebugModeSpawnsIgnoreTricksters ) {
				// Only one at a time
				if( Main.npc.Any(n => n?.active == true && n.type == ModContent.NPCType<TricksterNPC>()) ) {
					return false;
				}

				if( TricksterNPC.IsNearbyOtherTricksterDefeats( tileX, tileY ) ) {
					return false;
				}
			}

			return true;
		}

		public static bool CanSpawn_WorldGates_WeakRef( int tileX, int tileY ) {
			foreach( WorldGates.GateBarrier barrer in WorldGates.WorldGatesAPI.GetGateBarriers() ) {
				if( !barrer.IsActive ) {
					continue;
				}

				Rectangle area = barrer.WorldArea;
				area.X -= 3;
				area.Y -= 3;
				area.Width += 6;
				area.Height += 6;

				if( area.Contains(tileX, tileY) ) {
					return false;
				}
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

			if( TheTricksterMod.Instance.IsWorldGatesLoaded ) {
				if( !TricksterNPC.CanSpawn_WorldGates_WeakRef( tileX, tileY ) ) {
					return 0f;
				}
			}

			if( tileY < WorldLocationLibraries.SurfaceLayerBottomTileY ) {
				return TheTricksterConfig.Instance.Get<float>( nameof(TheTricksterConfig.SpawnChanceAboveGround) );
			} else {
				return TheTricksterConfig.Instance.Get<float>( nameof(TheTricksterConfig.SpawnChanceUnderGround) );
			}
		}


		////////////////
		
		public override int SpawnNPC( int tileX, int tileY ) {
			int npcWho = base.SpawnNPC( tileX, tileY );
			NPC myNpc = Main.npc[npcWho];

			if( myNpc?.active != true || myNpc.type != ModContent.NPCType<TricksterNPC>() ) {
				LogLibraries.Alert( "Could not spawn Trickster." );

				return npcWho;
			}

			//

			int nearPlrWho = myNpc.FindClosestPlayer();
			myNpc.target = nearPlrWho;

			var mynpc = myNpc.modNPC as TricksterNPC;
			TricksterState state = this.DecideOnSpawnState();

			//

			mynpc.SetState( state );

			return npcWho;
		}
	}
}
