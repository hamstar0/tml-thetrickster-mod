using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster {
	public class TheTricksterMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-thetrickster-mod";


		////////////////

		public static TheTricksterMod Instance { get; private set; }

		//public static TheTricksterConfig Config => ModContent.GetInstance<TheTricksterConfig>();


		////////////////

		private float FlashPercent = 0f;



		////////////////

		public TheTricksterMod() {
			TheTricksterMod.Instance = this;
		}

		////

		public override void Unload() {
			TheTricksterMod.Instance = null;
		}


		////////////////

		public override void PostDrawInterface( SpriteBatch sb ) {
			if( this.FlashPercent <= 0 ) {
				return;
			}

			float fade = (float)Math.Sin( (Math.PI * (double)this.FlashPercent) / 2d );

			sb.Draw(
				texture: Main.magicPixel,
				destinationRectangle: new Rectangle( 0, 0, Main.screenWidth, Main.screenHeight ),
				color: Color.White * fade
			);

			this.FlashPercent -= 0.5f / 60f;
		}


		////////////////

		public void DisplayFlash() {
			this.FlashPercent = 1f;
		}
	}
}