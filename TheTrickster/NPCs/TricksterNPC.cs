﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.World;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private bool IsAlerted = false;
		private bool IsDefeated = false;


		////////////////

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
			this.npc.defense = TheTricksterConfig.Instance.StatDefense;
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
			int baseHp = 5, addedHp = 0;

			if( TheTricksterConfig.Instance != null ) {
				baseHp = TheTricksterConfig.Instance.StatInitialLife;
				var myworld = ModContent.GetInstance<TheTricksterWorld>();

				if( myworld != null ) {
					addedHp = myworld.TricksterDefeats * TheTricksterConfig.Instance.StatLifeAddedEachDefeat;
				}
			}

			this.npc.lifeMax = Math.Min( baseHp + addedHp, TheTricksterConfig.Instance.StatLifeMax );
		}

		////////////////

		public override float SpawnChance( NPCSpawnInfo spawnInfo ) {
			// Underground only
			if( spawnInfo.spawnTileY < WorldHelpers.RockLayerTopTileY ) {
				return 0f;
			}
			// Only one at a time
			if( Main.npc.Any(n => n?.active == true && n.netID == ModContent.NPCType<TricksterNPC>()) ) {
				return 0f;
			}
			// Should have nearby NPCs
			if( spawnInfo.player.activeNPCs < 3f ) {
				return 0;
			}
			return TheTricksterConfig.Instance.SpawnChance;
		}

		////

		public override int SpawnNPC( int tileX, int tileY ) {
			int npcWho = base.SpawnNPC( tileX, tileY );
			NPC npc = Main.npc[npcWho];

			int nearPlrWho = npc.FindClosestPlayer();
			npc.target = nearPlrWho;

			return npcWho;
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
			if( !this.IsAlerted ) {
				this.IsAlerted = true;
				this.Encounter();
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

			this.RunFX();
			this.RunAI();

			base.AI();
		}


		////////////////

		public void Encounter() {
			Vector2 scrMid = Main.screenPosition;
			scrMid.X += Main.screenWidth / 2;
			scrMid.Y += Main.screenHeight / 2;
			float distSqr = Vector2.DistanceSquared( scrMid, this.npc.Center );

			if( distSqr < 409600 ) {
				Vector2 diff = this.npc.Center - scrMid;
				Vector2 pos = scrMid + ( diff * 0.5f );

				int soundSlot = this.mod.GetSoundSlot( SoundType.Custom, "Sounds/Custom/TricksterLaugh" );
				Main.PlaySound( (int)SoundType.Custom, (int)pos.X, (int)pos.Y, soundSlot );
			}
		}
	}
}