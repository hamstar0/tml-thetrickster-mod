using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.XNA;


namespace TheTrickster {
	class TheTricksterGlobalNPC : GlobalNPC {
		public override bool InstancePerEntity => true;


		////////////////

		public bool IsTricksterBat { get; internal set; } = false;



		////////////////
		
		public override void ModifyHitPlayer( NPC npc, Player target, ref int damage, ref bool crit ) {
			if( this.IsTricksterBat ) {
				damage = 1;
				crit = false;
				target.AddBuff( BuffID.Confused, 60 * 2 );

				npc.active = false;
			}
		}


		////////////////

		public override bool PreNPCLoot( NPC npc ) {
			return !this.IsTricksterBat;
		}


		////////////////

		public override Color? GetAlpha( NPC npc, Color drawColor ) {
			if( this.IsTricksterBat ) {
				return XNAColorHelpers.Mul( drawColor, Color.Red );
			}
			return base.GetAlpha( npc, drawColor );
		}

		public override void DrawEffects( NPC npc, ref Color drawColor ) {
			if( this.IsTricksterBat ) {
				if( Main.rand.Next( 60 ) < 8 ) {
					Dust.NewDust(
						Position: npc.Center,
						Width: 4,
						Height: 4,
						Type: 173  //21, 27, 62, 65
					);
				}
			}
		}
	}
}
