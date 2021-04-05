using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster {
	public partial class TheTricksterConfig : ModConfig {
		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( 60 * 180 )]
		public int MaxEncounterDurationTicks { get; set; } = 60 * 180;	// 3 minutes


		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( 60 * 2 )]
		public int IdleDurationTicks { get; set; } = 60 * 2;
		
		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( 60 * 30 )]
		public int LurkDurationTicks { get; set; } = 60 * 30;

		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( (int)(60f * 5.5f) )]
		public int AttackChargeupDurationTicks { get; set; } = (int)( 60f * 5.5f );

		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( 15 )]
		public int AttackChargeupDurationTicksReducedPerDefeat { get; set; } = 15;

		[Range( 1, 60 * 60 * 60 )]
		[DefaultValue( (int)(60f * 2.5f) )]
		public int AttackChargeupDurationTicksMinimum { get; set; } = (int)(60f * 2.5f);

		[Range( 0, 60 * 60 * 60 )]
		[DefaultValue( (int)( 60f * 6f ) )]
		public int CooldownDurationTicks { get; set; } = (int)( 60f * 6f );


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
		[DefaultValue( 60 * 10 )]
		public int InvulnTickDuration { get; set; } = 60 * 10;


		[Range( 1, 100 )]
		[DefaultValue( 1 )]
		public int HitsBeforeBlink { get; set; } = 1;

		[Range( 2, 60 * 60 * 10 )]
		[DefaultValue( 60 * 10 )]
		public int TricksterDefensiveBatTickDuration { get; set; } = 60 * 10;

		//[Range( 0, 100 )]
		//[DefaultValue( 2 )]
		//public int MaximumNearbyMobsBeforeFleeing { get; set; } = 2;


		[Range( 0, 64 * 16 )]
		[DefaultValue( 8 * 16 )]
		public int LurkStealRange { get; set; } = 8 * 16;
	}
}
