using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Fx;
using HamstarHelpers.Helpers.Items;
using HamstarHelpers.Helpers.TModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public static bool IsNearbyOtherTricksterDefeats( int tileX, int tileY ) {
			var config = TheTricksterConfig.Instance;
			var myworld = ModContent.GetInstance<TheTricksterWorld>();
			int minTileDist = config.Get<int>( nameof(config.MinTileSpawnDistanceAwayFromAnyPreviousDefeat) );
			int minTileDistSqr = minTileDist * minTileDist;

			foreach( (int dTileX, int dTileY) in myworld.TricksterDefeatLocations ) {
				int diffX = tileX - dTileX;
				int diffY = tileY - dTileY;
				int diffSqr = (diffX * diffX) + (diffY * diffY);

				if( diffSqr < minTileDistSqr ) {
					return true;
				}
			}

			return false;
		}



		////////////////

		public void DefeatEffects() {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			int soundSlot = this.mod.GetSoundSlot( SoundType.Custom, "Sounds/Custom/TricksterLaugh" );

			Main.PlaySound(
				type: (int)SoundType.Custom,
				x: (int)this.npc.Center.X,
				y: (int)this.npc.Center.Y,
				Style: soundSlot
			);
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 72, this.npc.width, this.npc.height );

			if( this.npc.target == Main.myPlayer ) {
				switch( rand.Next(6) ) {
				case 0:
					Main.NewText( "I'll be back!", Color.DarkMagenta );
					break;
				case 1:
					Main.NewText( "Ta ta!", Color.DarkMagenta );
					break;
				case 2:
					Main.NewText( "This isn't over!", Color.DarkMagenta );
					break;
				case 3:
					Main.NewText( "Hahahaha!", Color.DarkMagenta );
					break;
				case 4:
					Main.NewText( "Nice try!", Color.DarkMagenta );
					break;
				case 5:
					Main.NewText( "Whoops!", Color.DarkMagenta );
					break;
				}
			}

			if( Main.netMode != NetmodeID.MultiplayerClient ) {
				var config = TheTricksterConfig.Instance;
				var dropItemDef = config.Get<ItemDefinition>( nameof(config.DropsOnDefeat) );

				int dropItemType = dropItemDef?.Type ?? -1;
				if( dropItemType != -1 ) {
					ItemHelpers.CreateItem( this.npc.position, dropItemType, 1, 24, 24 );
				}
			}

			//Main.npc[this.npc.whoAmI] = new NPC();
			//this.npc.active = false;

			var myworld = ModContent.GetInstance<TheTricksterWorld>();
			myworld.AddTricksterDefeat( this.npc );
		}
	}
}
