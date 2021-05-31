using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;


namespace TheTrickster {
	public partial class TheTricksterMod : Mod {
		public static void InitializePKE() {
			PKEMeter.Logic.PKEGaugesGetter gauge = PKEMeter.PKEMeterAPI.GetGauge();

			float lastGaugedTricksterPercent = 0f;
			float lastFluctuationRate = 0f;

			int gaugeTimer = 0;

			//

			PKEMeter.PKEMeterAPI.SetGauge( ( plr, pos ) => {
				PKEMeter.Logic.PKEGaugeValues existingGauge = gauge?.Invoke( plr, pos )
					?? new PKEMeter.Logic.PKEGaugeValues( 0f, 0f, 0f, 0f);

				if( gaugeTimer-- <= 0 ) {
					gaugeTimer = 10;
					lastGaugedTricksterPercent = TheTricksterMod.GaugeTricksterPresence(
						worldPos: pos,
						fluctuationRate: out lastFluctuationRate
					);
				}

				// Yellow channel
				existingGauge.YellowPercent = TheTricksterMod.GetFluctuatedGaugeAmount(
					lastGaugedTricksterPercent,
					lastFluctuationRate
				);

				return existingGauge;
			} );

			//

			PKEMeter.PKEMeterAPI.SetMeterText( "Trickster", ( plr, pos, gauges ) => {
				string text = "";
				Color color = Color.White;
				float priority = 0f;

				if( lastGaugedTricksterPercent > 0f ) {
					color = Color.Yellow * ( 0.5f + ( Main.rand.NextFloat() * 0.5f ) );
					priority = 0.6f;

					if( Main.rand.NextFloat() < 0.9f ) {
						text = "CLASS VI TRANSDIM ELEVATED ORGANIC";
					} else {
						text = "CLASS IX ULTRADIM POST-MORTAL DEITY";
					}
				}

				return new PKEMeter.Logic.PKETextMessage( text, color, priority );
			} );

			PKEMeter.PKEMeterAPI.SetPKEYellowTooltip( () => "SPECIMEN" );
		}


		////////////////

		public static float GetFluctuatedGaugeAmount( float percent, float fluctuationRate ) {
			// No fluctuation this time?
			if( Main.rand.NextFloat() >= fluctuationRate ) {
				return percent;
			}

			float rand = Main.rand.NextFloat();
			float fluctuation = 1f - percent;
			fluctuation *= rand * rand;

			return percent + fluctuation;
		}
	}
}