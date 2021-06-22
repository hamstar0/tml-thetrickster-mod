using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.Players;
using TheTrickster.Packets;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public bool StealFromPlayer( Player player ) {
			if( Main.netMode == 1 ) {
				return true;
			}

			int randSlot;
			do {
				randSlot = Main.rand.Next( 0, PlayerItemLibraries.VanillaInventoryLastMainSlot );
			} while( player.inventory[randSlot].IsAir || player.selectedItem == randSlot );

			int itemIdx = PlayerItemLibraries.DropInventoryItem( player, randSlot );
			Item item = Main.item[ itemIdx ];
			if( item.IsAir ) {
				return false;
			}

			if( player.whoAmI == Main.myPlayer ) {
				string itemDesc = item.HoverName;
				if( item.stack > 1 ) {
					itemDesc = item.stack+" "+itemDesc+"s";
				} else {
					itemDesc = "A "+itemDesc;
				}

				Main.PlaySound( SoundID.Item14, this.npc.Center );
				Main.NewText( "Oh no! "+itemDesc+" was stolen from your inventory!", Color.OrangeRed );

				TheTricksterMod.Instance.DisplayFlash();
			}

			var myitem = item.GetGlobalItem<TheTricksterGlobalItem>();
			myitem.IsStolenBy = this.npc.whoAmI;

			if( Main.netMode == NetmodeID.Server ) {
				TricksterStealPacket.BroadcastToClients( this.npc.whoAmI, itemIdx );
			}

			return true;
		}
	}
}
