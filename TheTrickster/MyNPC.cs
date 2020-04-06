using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace TheTrickster {
	class TheTricksterGlobalNPC : GlobalNPC {
		public override bool InstancePerEntity => true;


		////////////////

		public bool IsTricksterBat = false;



		////////////////

		public override void ModifyHitPlayer( NPC npc, Player target, ref int damage, ref bool crit ) {
			if( this.IsTricksterBat ) {
				damage = 1;
				crit = false;
				target.AddBuff( BuffID.Confused, 60 * 2 );

				npc.active = false;
			}
		}
	}
}
