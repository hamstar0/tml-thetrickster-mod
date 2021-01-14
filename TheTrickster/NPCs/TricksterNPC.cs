using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private bool IsDefeated = false;


		////////////////
		
		public bool IsAlerted { get; private set; }

		public bool HasAttacked { get; private set; } = false;

		public int ElapsedTicksAlive { get; private set; } = 0;
		public int ElapsedStateTicks { get; private set; } = 0;
		public int HitsDuringCurrentStage { get; private set; } = 0;
		
		public TricksterState State { get; private set; } = TricksterState.Idle;


		////////////////

		public Player TargetPlayer => this.npc.HasPlayerTarget
			? (Main.player[this.npc.target]?.active == true ? Main.player[this.npc.target] : null)
			: null;

		////

		public override bool CloneNewInstances => false;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Trickster" );
			Main.npcFrameCount[ this.npc.type ] = 10;
		}
		
		public override void SetDefaults() {
			this.SetDefaultMaxLife();
			this.npc.defense = TheTricksterConfig.Instance.Get<int>( nameof(TheTricksterConfig.StatDefense) );
			this.npc.width = 18;
			this.npc.height = 40;
			this.npc.damage = 14;
			this.npc.HitSound = SoundID.NPCHit1;
			//this.npc.DeathSound = SoundID.NPCDeath2;
			this.npc.value = 60f;
			this.npc.knockBackResist = 0.1f;
			this.npc.aiStyle = -1;//8
			this.npc.lavaImmune = true;
			this.animationType = NPCID.FireImp;
			this.banner = Item.NPCtoBanner( NPCID.FireImp );
			this.bannerItem = Item.BannerToItem( this.banner );

			//this.npc.buffImmune[BuffID.OnFire] = true;
			//this.npc.buffImmune[BuffID.CursedInferno] = true;
			for( int i = 0; i < this.npc.buffImmune.Length; i++ ) {
				this.npc.buffImmune[i] = true;
			}
		}

		public override void ScaleExpertStats( int numPlayers, float bossLifeScale ) {
			this.SetDefaultMaxLife();
		}

		private void SetDefaultMaxLife() {
			var config = TheTricksterConfig.Instance;
			int baseHp = 5, addedHp = 0;
			int statLifeMax = config.Get<int>( nameof( TheTricksterConfig.StatLifeMax ) );

			if( TheTricksterConfig.Instance != null ) {
				baseHp = config.Get<int>( nameof(TheTricksterConfig.StatInitialLife) );
				var myworld = ModContent.GetInstance<TheTricksterWorld>();

				if( myworld != null ) {
					int lifeAdded = config.Get<int>( nameof(TheTricksterConfig.StatLifeAddedEachDefeat) );
					addedHp = myworld.TricksterDefeatLocations.Count * lifeAdded;
				}
			}

			this.npc.lifeMax = Math.Min( baseHp + addedHp, statLifeMax );
		}


		////////////////

		public override bool CheckDead() {
			if( !this.IsDefeated ) {
				this.IsDefeated = true;
				this.DefeatEffects();
			}
			return base.CheckDead();
		}


		////////////////

		public override bool StrikeNPC( ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit ) {
			if( damage > 0 && this.npc.life > 1 ) {
				this.RunOnHitAI();
			}
			return true;
		}


		////////////////

		public override void AI() {
			bool isNewlyAlerted = false;

			if( !this.IsAlerted ) {
				this.IsAlerted = true;
				isNewlyAlerted = true;
			}

			Player targPlr = this.TargetPlayer;
			if( targPlr == null || targPlr.dead ) {
				if( Vector2.DistanceSquared( targPlr.Center, this.npc.Center ) > 10240000 ) {  // 200 tiles
					this.npc.target = this.npc.FindClosestPlayer();
				}
			}

			// Resist being knocked around when on the ground
			if( this.npc.velocity.X != 0 && this.npc.velocity.Y == 0 ) {
				this.npc.velocity.X *= 0.9f;
			}

			this.RunFX( isNewlyAlerted );
			this.RunAI();

			base.AI();
		}


		////////////////

		public override Color? GetAlpha( Color drawColor ) {
			if( this.State == TricksterState.Lurk ) {
				return drawColor * 0.15f;
			}
			return base.GetAlpha( drawColor );
		}
	}
}
