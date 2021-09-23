using System;
using System.Collections.Generic;
using Terraria.ModLoader;


namespace TheTrickster {
	public class TheTricksterAPI {
		public static ISet<(int tileX, int tileY)> GetTricksterDefeatLocations() {
			var myworld = ModContent.GetInstance<TheTricksterWorld>();

			return new HashSet<(int, int)>( myworld.TricksterDefeatLocations );
		}
	}
}
