using Terraria.ModLoader;


namespace TheTrickster {
	public class TheTricksterMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-thetrickster-mod";


		////////////////

		public static TheTricksterMod Instance { get; private set; }

		public static TheTricksterConfig Config => ModContent.GetInstance<TheTricksterConfig>();



		////////////////

		public TheTricksterMod() {
			TheTricksterMod.Instance = this;
		}

		////

		public override void Unload() {
			TheTricksterMod.Instance = null;
		}
	}
}