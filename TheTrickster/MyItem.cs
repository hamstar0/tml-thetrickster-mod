using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using TheTrickster.NPCs;


namespace TheTrickster {
	class TheTricksterGlobalItem : GlobalItem {
		public int IsStolenBy = -1;


		////////////////

		public override bool InstancePerEntity => true;



		////////////////

		public override void Update( Item item, ref float gravity, ref float maxFallSpeed ) {
			if( this.IsStolenBy != -1 ) {
				NPC npc = Main.npc[ this.IsStolenBy ];
				if( npc?.active != true || npc.type != ModContent.NPCType<TricksterNPC>() ) {
					this.IsStolenBy = -1;
					return;
				}

				if( item.noGrabDelay < 3 ) {
					item.noGrabDelay = 3;
				}

				gravity = 0f;
				item.velocity = Vector2.Zero;
				item.Center = npc.Center;
			}
		}
	}
}
