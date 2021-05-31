using System;
using Terraria.ModLoader.Config;
using ModLibsCore.Classes.UI.ModConfig;
using ModLibsCore.Libraries.Debug;


namespace TheTrickster {
	class MyFloatInputElement : FloatInputElement { }



	
	public partial class TheTricksterConfig : ModConfig {
		public static TheTricksterConfig Instance { get; internal set; }



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;



		////////////////

		public override ModConfig Clone() {
			var clone = (TheTricksterConfig)this.MemberwiseClone();
			clone.DropsOnDefeat = new ItemDefinition( this.DropsOnDefeat.Type );
			return clone;
		}
	}
}
