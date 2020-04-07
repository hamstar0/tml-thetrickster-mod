using System;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;
using HamstarHelpers.Classes.UI.ModConfig;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Configs;


namespace TheTrickster {
	class MyFloatInputElement : FloatInputElement { }



	
	public partial class TheTricksterConfig : StackableModConfig {
		public static TheTricksterConfig Instance => ModConfigStack.GetMergedConfigs<TheTricksterConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;


		////////////////

		[Range( 0f, 10f )]
		[DefaultValue( 0.05f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float SpawnChance { get; set; } = 0.05f;	//0.02?

		[Range( 1, 10000 )]
		[DefaultValue( 4 )]
		public int StatInitialLife { get; set; } = 4;

		[Range( 0, 100000 )]
		[DefaultValue( 1 )]
		public int StatLifeAddedEachDefeat { get; set; } = 1;

		[Range( 0, 100000 )]
		[DefaultValue( 15 )]
		public int StatLifeMax { get; set; } = 15;

		[Range( 0, 999999 )]
		[DefaultValue( 999999 )]
		public int StatDefense { get; set; } = 999999;

		public ItemDefinition DropsOnDefeat { get; set; } = new ItemDefinition( ItemID.TeleportationPotion );

		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( 60 * 180 )]
		public int TicksUntilFlee { get; set; } = 60 * 180;


		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( 60 * 2 )]
		public int IdleDurationTicks { get; set; } = 60 * 2;
		
		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( 60 * 20 )]
		public int LurkDurationTicks { get; set; } = 60 * 20;

		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( (int)(60f * 5.5f) )]
		public int AttackDurationTicks { get; set; } = (int)( 60f * 5.5f );

		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( 15 )]
		public int AttackDurationTicksReducedPerDefeat { get; set; } = 15;

		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( 60 * 2 )]
		public int AttackDurationTicksMinimum { get; set; } = 60 * 2;

		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( (int)( 60f * 1.5f ) )]
		public int CooldownDurationTicks { get; set; } = (int)( 60f * 1.5f );


		[Range( 4 * 16, 1000 * 16 )]
		[DefaultValue( 48 * 16 )]
		public int AttackRadius { get; set; } = 48 * 16;

		[Range( 4 * 16, 1000 * 16 )]
		[DefaultValue( 16 * 16 )]
		public int MinDodgeRadius { get; set; } = 16 * 16;

		[Range( 4 * 16, 1000 * 16 )]
		[DefaultValue( 44 * 16 )]
		public int MaxDodgeRadius { get; set; } = 44 * 16;

		[Range( 60, 60 * 60 * 60 )]
		[DefaultValue( 60 * 15 )]
		public int InvulnTickDuration { get; set; } = 60 * 15;


		[Range( 1, 100 )]
		[DefaultValue( 1 )]
		public int HitsBeforeBlink { get; set; } = 1;

		//[Range( 0, 100 )]
		//[DefaultValue( 2 )]
		//public int MaximumNearbyMobsBeforeFleeing { get; set; } = 2;


		[Range( 0, 64 * 16 )]
		[DefaultValue( 8 * 16 )]
		public int LurkStealRange { get; set; } = 8 * 16;



		////////////////

		public override ModConfig Clone() {
			var clone = (TheTricksterConfig)this.MemberwiseClone();
			clone.DropsOnDefeat = this.DropsOnDefeat;
			return clone;
		}
	}
}
