using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TheTrickster.NPCs;


namespace TheTrickster {
	public partial class TheTricksterMod : Mod {
		public static void InitializePKE() {
			PKEMeter.Logic.PKEText meterTextFunc = PKEMeter.PKEMeterAPI.GetMeterText();
			PKEMeter.Logic.PKEGauge gauge = PKEMeter.PKEMeterAPI.GetGauge();
			float lastGaugedTricksterPercent = 0f;
			float lastFluctuationRate = 0f;
			int timer = 0;

			PKEMeter.PKEMeterAPI.SetGauge( ( plr, pos ) => {
				(float b, float g, float y, float r) existingGauge = gauge?.Invoke( plr, pos )
					?? (0f, 0f, 0f, 0f);

				if( timer-- <= 0 ) {
					timer = 10;
					lastGaugedTricksterPercent = TheTricksterMod.GaugeTricksterPresence( pos, out lastFluctuationRate )
						?? 0f;
				}

				// Yellow channel
				existingGauge.y = TheTricksterMod.ApplyFluctuation( lastGaugedTricksterPercent, lastFluctuationRate );

				return existingGauge;
			} );

			PKEMeter.PKEMeterAPI.SetMeterText( ( plr, pos, gauges ) => {
				(string text, Color color) currText = meterTextFunc?.Invoke( plr, pos, gauges )
					?? ("", Color.Transparent);

				if( gauges.r > 0.75f ) {
				} else if( gauges.y > 0.75f ) {
					currText.color = Color.Yellow;
					if( Main.rand.NextFloat() < 0.99f ) {
						currText.text = "CLASS VI TRANSDIM ELEVATED ORGANIC";
					} else {
						currText.text = "CLASS IX ULDTRADIM POST-MORTAL DEITY";
					}
				}

				currText.color = currText.color * (0.5f + (Main.rand.NextFloat() * 0.5f));

				return currText;
			} );
		}

		////

		public static float? GaugeTricksterPresence( Vector2 worldPos, out float fluctuationRate ) {
			var config = TheTricksterConfig.Instance;
			int maxTileRange = config.Get<int>( nameof( config.TricksterPKEDetectionTileRangeMax ) );
			if( maxTileRange <= 0 ) {
				fluctuationRate = 0f;
				return null;
			}

			int tileX = (int)worldPos.X / 16;
			int tileY = (int)worldPos.Y / 16;
			if( !TricksterNPC.IsWithinSpawnRange( tileX, tileY ) ) {
				fluctuationRate = 0f;
				return null;
			}

			if( TricksterNPC.IsNearbyOtherTricksterDefeats( tileX, tileY ) ) {
				fluctuationRate = 0f;
				return null;
			}

			NPC nearest = TheTricksterMod.FindNearestTrickster( worldPos, maxTileRange );

			return TheTricksterMod.GaugeTrickstersNear( worldPos, maxTileRange, nearest, out fluctuationRate );
		}


		public static float GaugeTrickstersNear(
					Vector2 worldPos,
					int maxTileRange,
					NPC nearestTricksterNPC,
					out float fluctuationRate ) {
			float maxWldRange = maxTileRange * 16f;
			float nearestNpcDist = (nearestTricksterNPC.Center - worldPos).Length();

			float nearestPerc = (float)nearestNpcDist / (float)maxWldRange;
			float gaugedPerc = Math.Max( 1f - nearestPerc, 0f );

			var mynpc = (TricksterNPC)nearestTricksterNPC.modNPC;

			if( nearestTricksterNPC != null ) {
				// Apply 'is alerted' fluctuations
				if( mynpc.IsAlerted ) {
					fluctuationRate = 1f;
				}
				// Apply 'is lurking nearby' fluctuations
				else {
					fluctuationRate = 1f / 60f;
				}
			}
			// Apply 'is able to spawn' fluctuations
			else {
				fluctuationRate = 10f / 60f;
			}

			return gaugedPerc;
		}


		public static float ApplyFluctuation( float percent, float fluctuationRate ) {
			if( Main.rand.NextFloat() < fluctuationRate ) {
				return percent;
			}

			float fluctuation = 1f - percent;
			fluctuation *= Main.rand.NextFloat();

			return percent + fluctuation;
		}


		////////////////

		public static NPC FindNearestTrickster( Vector2 worldPos, int maxTileRange ) {
			NPC nearestNpc = null;

			int tricksterType = ModContent.NPCType<TricksterNPC>();

			int maxRange = maxTileRange * 16;
			int maxRangeSqr = maxRange * maxRange;
			int nearestDistSqr = Int32.MaxValue;

			int maxNpcs = Main.npc.Length;
			for( int i = 0; i < maxNpcs; i++ ) {
				NPC npc = Main.npc[i];
				if( npc?.active != true || npc.type != tricksterType ) {
					continue;
				}

				int distSqr = (int)( worldPos - npc.Center ).LengthSquared();
				if( distSqr >= maxRangeSqr ) {
					continue;
				}

				if( distSqr < nearestDistSqr ) {
					nearestDistSqr = distSqr;
					nearestNpc = npc;
				}
			}

			return nearestNpc;
		}
	}
}