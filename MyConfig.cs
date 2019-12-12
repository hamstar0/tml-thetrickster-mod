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

		[Range( 0f, 10f )]
		[DefaultValue( 0.02f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float TricksterSpawnChance { get; set; } = 0.02f;

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
		[DefaultValue( 60 * 180 )]
		public int TicksUntilFlee { get; set; } = 60 * 180;


		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( 60 * 2 )]
		public int IdleDurationTicks { get; set; } = 60 * 2;

		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( (int)(60f * 6.5f) )]
		public int AttackDurationTicks { get; set; } = (int)(60f * 6.5f);

		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( (int)( 60f * 1.5f ) )]
		public int CooldownDurationTicks { get; set; } = (int)( 60f * 1.5f );


		[Range( 4 * 16, 1000 * 16 )]
		[DefaultValue( 48 * 16 )]
		public int AttackRadius { get; set; } = 48 * 16;

		[Range( 4 * 16, 1000 * 16 )]
		[DefaultValue( 36 * 16 )]
		public int DodgeRadius { get; set; } = 36 * 16;

		[Range( 60, 60 * 60 * 60 )]
		[DefaultValue( 60 * 15 )]
		public int InvulnTickDuration { get; set; } = 60 * 15;



		////////////////

		public override ModConfig Clone() {
			var clone = (TheTricksterConfig)this.MemberwiseClone();
			clone.DropsOnDefeat = this.DropsOnDefeat;
			return clone;
		}
	}
}
