﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Fx;
using HamstarHelpers.Helpers.Items;
using HamstarHelpers.Helpers.TModLoader;
using Terraria.ModLoader.Config;

namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void DefeatEffects() {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			int soundSlot = this.mod.GetSoundSlot( SoundType.Custom, "Sounds/Custom/TricksterLaugh" );

			Main.PlaySound( (int)SoundType.Custom, (int)this.npc.Center.X, (int)this.npc.Center.Y, soundSlot );
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

			if( Main.netMode != 1 ) {
				var drops = TheTricksterConfig.Instance.Get<ItemDefinition>( nameof(TheTricksterConfig.DropsOnDefeat) );
				int itemType = drops?.Type ?? -1;
				if( itemType != -1 ) {
					ItemHelpers.CreateItem( this.npc.position, itemType, 1, 24, 24 );
				}
			}

			//Main.npc[this.npc.whoAmI] = new NPC();
			//this.npc.active = false;

			var myworld = ModContent.GetInstance<TheTricksterWorld>();
			myworld.AddTricksterDefeat();
		}
	}
}
