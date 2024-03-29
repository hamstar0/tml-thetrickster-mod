﻿using System;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public TricksterState DecideOnSpawnState() {
			var myworld = ModContent.GetInstance<TheTricksterWorld>();

			if( myworld.TricksterEncounters <= 3 ) {
				return TricksterState.Mock;
			}

			var config = TheTricksterConfig.Instance;
			float passivePerc = config.Get<float>( nameof(config.OnSpawnIsPassivePercentChance) );

			if( Main.rand.NextFloat() < passivePerc ) {
				return TricksterState.Mock;
			}

			return TricksterState.Idle;
		}
	}
}
