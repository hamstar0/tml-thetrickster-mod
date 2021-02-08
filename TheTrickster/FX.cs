using System;
using Microsoft.Xna.Framework;
using Terraria;
using HamstarHelpers.Helpers.Fx;
using HamstarHelpers.Services.Hooks.Draw;


namespace TheTrickster {
	public partial class FX {
		public static void TricksterChargeSpark( Vector2 position, Vector2 velocity ) {
			Dust dust = Dust.NewDustPerfect(
				Position: position,
				Type: 222,//	133:0.5, 222:0.5, 246
				Velocity: velocity,
				Scale: 0.5f
			);
			dust.noGravity = true;
			dust.fadeIn = 0.4f;//0.33f;
		}


		public static void TricksterChargeArc(
					Vector2 position,
					Vector2 direction,
					float length,
					float scaleMin = 0.01f,
					float scaleMax = 0.11f ) {
			int duration = 3;

			DrawHooks.AddPostDrawTilesHook( () => {
				float scaleRand = Main.rand.NextFloat();
				float scale = scaleMin + (scaleRand * (scaleMax-scaleMin));
				Color color = Color.White * (0.5f + (scaleRand * 0.5f));

				LightningFxHelpers.DrawLightning(
					position,
					position + (direction * length),
					scale,
					color
				);
				return duration-- > 0;
			} );
		}


		public static void TricksterAttackSpark( Vector2 position ) {
			int dustIdx = Dust.NewDust(
				Position: position,
				Width: 1,
				Height: 1,
				Type: 269,
				SpeedX: 0f,
				SpeedY: 0f,
				Alpha: 0,
				newColor: Color.White,
				Scale: 1f
			);
			Dust dust = Main.dust[dustIdx];
			dust.noGravity = true;
		}


		public static void BatDust( Vector2 position ) {
			Dust.NewDust(
				Position: position,
				Width: 4,
				Height: 4,
				Type: 173  //21, 27, 62, 65
			);
		}
	}
}
