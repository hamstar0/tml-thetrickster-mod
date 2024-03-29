﻿using System;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void EncounterFormal( bool makesLaugh ) {
			var myworld = ModContent.GetInstance<TheTricksterWorld>();
			myworld.AddTricksterEncounter();

			if( makesLaugh ) {
				FX.TricksterLaughFX( this.npc.Center );
			}
		}
	}
}
