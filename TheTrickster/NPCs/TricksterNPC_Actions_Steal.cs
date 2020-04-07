using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public bool StealFromPlayer( Player player ) {
			if( Main.netMode == 1 ) {
				return true;
			}

			int randSlot;
			do {
				randSlot = Main.rand.Next( 0, PlayerItemHelpers.VanillaInventoryLastMainSlot );
			} while( player.inventory[randSlot].IsAir || player.selectedItem == randSlot );

			int itemIdx = PlayerItemHelpers.DropInventoryItem( player, randSlot );
			Item item = Main.item[ itemIdx ];
			if( item.IsAir ) {
				return false;
			}

			if( player.whoAmI == Main.myPlayer ) {
				Main.PlaySound( SoundID.Item14, this.npc.Center );
				Main.NewText( item.HoverName + " was stolen from your inventory!", Color.Red );

				TheTricksterMod.Instance.DisplayFlash();
			}

			var myitem = item.GetGlobalItem<TheTricksterGlobalItem>();
			myitem.IsStolenBy = this.npc.whoAmI;

			if( Main.netMode == 2 ) {
				TricksterStealProtocol.Broadcast( this.npc.whoAmI, itemIdx );
			}

			return true;
		}
	}
}
