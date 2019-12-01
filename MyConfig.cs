using HamstarHelpers.Classes.UI.ModConfig;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Configs;
using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;


namespace TheTrickster {
	class MyFloatInputElement : FloatInputElement { }



	
	public partial class TheTricksterConfig : StackableModConfig {
		public static TheTricksterConfig Instance => ModConfigStack.GetMergedConfigs<TheTricksterConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;


		////////////////

		[Range( 0f, 20f )]
		[DefaultValue( 0.005f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float TricksterSpawnChance { get; set; } = 0.005f;

		[Range( 1, 100000 )]
		[DefaultValue( 5 )]
		public int TricksterStatInitialLife { get; set; } = 5;

		[Range( 0, 100000 )]
		[DefaultValue( 1 )]
		public int TricksterStatLifeAddedEachDefeat { get; set; } = 1;

		[Range( 0, 999999 )]
		[DefaultValue( 999999 )]
		public int TricksterStatDefense { get; set; } = 999999;

		public ItemDefinition DropsOnDefeat { get; set; } = new ItemDefinition( ItemID.TeleportationPotion );

		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( 60 * 60 )]
		public int TicksUntilFlee { get; set; } = 60 * 60;

		

		////////////////

		public override ModConfig Clone() {
			var clone = (TheTricksterConfig)this.MemberwiseClone();
			clone.DropsOnDefeat = this.DropsOnDefeat;
			return clone;
		}
	}
}
