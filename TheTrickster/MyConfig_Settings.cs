using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;
using ModLibsCore.Libraries.Debug;


namespace TheTrickster {
	public partial class TheTricksterConfig : ModConfig {
		public bool DebugModeInfo { get; set; } = false;

		public bool DebugModeSpawnsIgnoreRange { get; set; } = false;

		public bool DebugModeSpawnsIgnoreLocation { get; set; } = false;

		public bool DebugModeSpawnsIgnoreNpcs { get; set; } = false;

		public bool DebugModeSpawnsIgnoreTricksters { get; set; } = false;


		////

		[Range( 0f, 10f )]
		[DefaultValue( 0.05f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float SpawnChanceAboveGround { get; set; } = 0.05f; // was 0.015

		[Range( 0f, 10f )]
		[DefaultValue( 0.1f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float SpawnChanceUnderGround { get; set; } = 0.1f;	// was 0.03

		//

		[Range( 0, 100 )]
		[DefaultValue( 2 )]	// was 3
		public int RequiredNearbyNpcsForSpawn { get; set; } = 2;

		//

		[Range( 0f, 1f )]
		[DefaultValue( 1f / 3f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float OnSpawnIsPassivePercentChance { get; set; } = 1f / 3f;

		//

		[Range( 0, 8192 )]
		[DefaultValue( 192 )]
		public int MinTileSpawnDistanceAwayFromAnyPreviousDefeat { get; set; } = 192;


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
