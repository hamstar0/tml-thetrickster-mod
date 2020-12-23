using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;


namespace TheTrickster {
	public partial class TheTricksterMod : Mod {
		public static void InitializePKE() {
			PKEMeter.Logic.PKEText meterTextFunc = PKEMeter.PKEMeterAPI.GetMeterText();
			PKEMeter.Logic.PKEGauge gauge = PKEMeter.PKEMeterAPI.GetGauge();

			float lastGaugedTricksterPercent = 0f;
			float lastFluctuationRate = 0f;
			bool isTricksterNear = false;

			int gaugeTimer = 0;
			int textTimer = 0;

			//

			PKEMeter.PKEMeterAPI.SetGauge( ( plr, pos ) => {
				(float b, float g, float y, float r) existingGauge = gauge?.Invoke( plr, pos )
					?? (0f, 0f, 0f, 0f);

				if( gaugeTimer-- <= 0 ) {
					gaugeTimer = 10;
					lastGaugedTricksterPercent = TheTricksterMod.GaugeTricksterPresence(
						pos, out lastFluctuationRate, out isTricksterNear
					) ?? 0f;
				}

				// Yellow channel
				existingGauge.y = TheTricksterMod.ApplyFluctuation( lastGaugedTricksterPercent, lastFluctuationRate );

				return existingGauge;
			} );


			PKEMeter.PKEMeterAPI.SetMeterText( ( plr, pos, gauges ) => {
				(string text, Color color) currText = meterTextFunc?.Invoke( plr, pos, gauges )
					?? ("", Color.Transparent);

				textTimer--;

				if( !isTricksterNear && currText.text != "" ) {   // yield when no specific signals
					return currText;
				}

				if( textTimer <= 0 && gauges.y > 0.75f ) {
					textTimer = 60;
				}

				if( textTimer > 0 ) {
					currText.color = Color.Yellow * ( 0.5f + ( Main.rand.NextFloat() * 0.5f ) );

					if( Main.rand.NextFloat() < 0.9f ) {
						currText.text = "CLASS VI TRANSDIM ELEVATED ORGANIC";
					} else {
						currText.text = "CLASS IX ULDTRADIM POST-MORTAL DEITY";
					}
				}

				return currText;
			} );
		}
	}
}