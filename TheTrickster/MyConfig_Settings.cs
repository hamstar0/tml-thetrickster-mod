using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster {
	public partial class TheTricksterConfig : ModConfig {
		[Range( 0f, 10f )]
		[DefaultValue( 0.04f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float SpawnChanceAboveGround { get; set; } = 0.04f;
		
		[Range( 0f, 10f )]
		[DefaultValue( 0.02f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float SpawnChanceUnderGround { get; set; } = 0.02f;	// 0.03?

		//
		
		[Range( 0f, 1f )]
		[DefaultValue( 0.2f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float OnSpawnPassivePercentChance { get; set; } = 0.2f;

		//

		[Range( 0, 8192 )]
		[DefaultValue( 128 )]
		public int MinTileSpawnDistanceAwayFromAnyPreviousDefeat { get; set; } = 128;


		//

		[Range( 1, 10000 )]
		[DefaultValue( 4 )]
		public int StatInitialLife { get; set; } = 4;

		[Range( 0, 100000 )]
		[DefaultValue( 1 )]
		public int StatLifeAddedEachDefeat { get; set; } = 1;

		[Range( 0, 100000 )]
		[DefaultValue( 13 )]
		public int StatLifeMax { get; set; } = 13;

		[Range( 0, 999999 )]
		[DefaultValue( 999999 )]
		public int StatDefense { get; set; } = 999999;


		//

		public ItemDefinition DropsOnDefeat { get; set; } = new ItemDefinition( ItemID.TeleportationPotion );


		////

		[Range( -1, 4048 )]
		[DefaultValue( 64 )]
		public int TricksterPKEDetectionTileRangeMax { get; set; } = 64;
	}
}
