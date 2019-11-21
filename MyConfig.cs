using HamstarHelpers.Classes.UI.ModConfig;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace TheTrickster {
	class MyFloatInputElement : FloatInputElement { }




	public partial class TheTricksterConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;



		////

		[Range( 0f, 100f )]
		[DefaultValue( 0.01f )]
		public float TricksterSpawnChance { get; set; } = 0.01f;



		////////////////

		/*public override ModConfig Clone() {
			var clone = (AdventureModeConfig)base.Clone();
			return clone;
		}*/
	}
}
