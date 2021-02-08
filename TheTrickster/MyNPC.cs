using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.NPCs;
using HamstarHelpers.Helpers.XNA;


namespace TheTrickster {
	class TheTricksterGlobalNPC : GlobalNPC {
		public override bool InstancePerEntity => true;


		////////////////

		public int TricksterBatDurationTicks { get; internal set; } = 0;



		////////////////

		public override bool PreAI( NPC npc ) {
			if( this.TricksterBatDurationTicks > 1 ) {
				this.TricksterBatDurationTicks--;
			}

			if( this.TricksterBatDurationTicks == 1 ) {
				NPCHelpers.RawHurt( npc, 9999 );
			}

			return base.PreAI( npc );
		}


		////////////////

		public override void ModifyHitPlayer( NPC npc, Player target, ref int damage, ref bool crit ) {
			if( this.TricksterBatDurationTicks > 0 ) {
				damage = 1;
				crit = false;
				target.AddBuff( BuffID.Confused, 60 * 2 );

				npc.active = false;
			}
		}


		////////////////

		public override bool PreNPCLoot( NPC npc ) {
			return this.TricksterBatDurationTicks <= 0;
		}


		////////////////

		public override Color? GetAlpha( NPC npc, Color drawColor ) {
			if( this.TricksterBatDurationTicks > 0 ) {
				return XNAColorHelpers.Mul( drawColor, Color.Red );
			}
			return base.GetAlpha( npc, drawColor );
		}

		public override void DrawEffects( NPC npc, ref Color drawColor ) {
			if( this.TricksterBatDurationTicks > 0 ) {
				if( Main.rand.Next( 60 ) < 8 ) {
					FX.BatDust( npc.Center );
				}
			}
		}
	}
}
