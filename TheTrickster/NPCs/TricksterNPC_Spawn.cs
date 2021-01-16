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
			if( TricksterNPC.IsWithinSpawnRange(spawnInfo.spawnTileX, spawnInfo.spawnTileY) ) {
				return 0f;
			}

			// Should have nearby NPCs
			if( spawnInfo.player.activeNPCs < 3f ) {
				return 0f;
			}

			/*spawnInfo.player.ZoneJungle ||*/
			if( spawnInfo.player.ZoneHoly || spawnInfo.player.ZoneCorrupt || spawnInfo.player.ZoneCrimson ) {
				return 0f;
			}

			if( spawnInfo.player.ZoneSandstorm ) {
				return 0f;
			}

			// Only one at a time
			if( Main.npc.Any(n => n?.active == true && n.netID == ModContent.NPCType<TricksterNPC>()) ) {
				return 0f;
			}

			if( TricksterNPC.IsNearbyOtherTricksterDefeats(spawnInfo.spawnTileX, spawnInfo.spawnTileY) ) {
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
