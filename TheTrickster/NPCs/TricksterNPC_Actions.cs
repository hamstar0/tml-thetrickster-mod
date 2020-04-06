﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using HamstarHelpers.Buffs;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Fx;
using HamstarHelpers.Helpers.Items;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Helpers.World;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public void LaunchAttack() {
			this.HasAttacked = true;

			this.CreateLaunchedAttackFX( TheTricksterConfig.Instance.AttackRadius );

			int radiusSqr = TheTricksterConfig.Instance.AttackRadius * TheTricksterConfig.Instance.AttackRadius;
			int invulnBuffType = ModContent.BuffType<DegreelessnessBuff>();

			for( int i=0; i<Main.npc.Length; i++ ) {
				NPC otherNpc = Main.npc[i];
				if( otherNpc?.active != true || otherNpc.friendly || otherNpc.immortal ) {
					continue;
				}
				if( otherNpc.whoAmI == this.npc.whoAmI || otherNpc.type == this.npc.type ) {
					continue;
				}

				if( Vector2.DistanceSquared(otherNpc.Center, this.npc.Center) < radiusSqr ) {
					var mynpc = otherNpc.GetGlobalNPC<TheTricksterGlobalNPC>();
					if( mynpc.IsTricksterBat ) {
						continue;
					}

					otherNpc.AddBuff( invulnBuffType, TheTricksterConfig.Instance.InvulnTickDuration );
				}
			}
		}


		////////////////

		public void DodgeAction( int minDodgeRadius, int maxDodgeRadius ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			int minDistSqr = minDodgeRadius * minDodgeRadius;

			int plrWho = this.npc.HasPlayerTarget
					? this.npc.target
					: this.npc.FindClosestPlayer();
			if( plrWho == -1 ) {
				return;
			}

			Player player = Main.player[ plrWho ];
			if( player == null || !player.active ) {
				return;
			}

			Vector2 dir, testPos, groundPos;
			bool isOnGround;
			int tileX=0, tileY=0;
			do {
				dir = new Vector2( rand.NextFloat() - 0.5f, rand.NextFloat() - 0.5f );
				dir.Normalize();
				dir *= minDodgeRadius + ( rand.NextFloat() * (maxDodgeRadius - minDodgeRadius ) );

				testPos = player.Center + dir;
				maxDodgeRadius += 1;

				isOnGround = WorldHelpers.DropToGround( testPos, false, TilePattern.CommonSolid, out groundPos );
				if( !isOnGround ) { continue; }

				if( Vector2.DistanceSquared(player.Center, groundPos) < minDistSqr ) {
					isOnGround = false;
					continue;
				}

				tileX = (int)groundPos.X / 16;
				tileY = (int)groundPos.Y / 16;
			} while( !isOnGround || !TilePattern.NonSolid.CheckArea( new Rectangle(tileX-1, tileY-3, 3, 3) ) );

			// Before
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 48, this.npc.width, this.npc.height );

			if( Main.netMode != 1 ) {
				this.npc.position = groundPos - new Vector2( 0, this.npc.height + 1 );

				if( Main.netMode == 2 ) {
					NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, this.npc.whoAmI );
				}
			}

			// After
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 48, this.npc.width, this.npc.height );
		}

		////

		public void DeployDefenseBats() {
			if( Main.netMode != 1 ) {
				for( int i=0; i<2; i++ ) {
					this.DeployDefenseBat();
				}
			}

			if( Main.netMode != 2 ) {
				ParticleFxHelpers.MakeDustCloud( this.npc.Center, 3, 0.3f, 2f );
			}
		}

		private void DeployDefenseBat() {
			int npcWho = NPC.NewNPC( (int)this.npc.position.X, (int)this.npc.position.Y, NPCID.CaveBat, 0, 0f, 0f, 0f, 0f, this.npc.target );
			NPC npc = Main.npc[npcWho];
			if( !npc.active ) {
				return;
			}

			npc.scale = 0.65f;
			npc.life = 1;
			npc.lifeMax = 1;
			npc.defense = 999999;
			npc.damage = 1;
			//npc.color = XNAColorHelpers.Mul( npc.color, Color.Red );
			npc.value = 0;
			npc.SpawnedFromStatue = true;	// no loot abuse?
			npc.velocity = new Vector2(
				Main.rand.NextFloat() - 0.5f,
				Main.rand.NextFloat() - 0.5f
			) * 8f;

			var mynpc = npc.GetGlobalNPC<TheTricksterGlobalNPC>();
			mynpc.IsTricksterBat = true;

			if( Main.netMode == 2 ) {
				NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, npc.whoAmI );
				TricksterBatProtocol.Broadcast( npcWho );
			}

			npc.AddBuff( ModContent.BuffType<DegreelessnessBuff>(), 60 );
		}


		////

		public void FleeAction() {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			Vector2 pos = this.npc.position;

			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 72, this.npc.width, this.npc.height );

			if( this.TargetPlayer != null && this.State != TricksterState.Idle ) {
				for( int i = 0; i < 3; i++ ) {
					float dirX = 6f * ( rand.NextFloat() - 0.5f );

					Projectile proj = Main.projectile[Projectile.NewProjectile(
						X: pos.X,
						Y: pos.Y,
						SpeedX: dirX,
						SpeedY: -2f,
						Type: ProjectileID.HappyBomb,
						Damage: 0,
						KnockBack: 0f,
						Owner: Main.myPlayer
					)];
					proj.timeLeft = 150;
				}
			}

			Main.npc[this.npc.whoAmI] = new NPC();
			this.npc.active = false;

			if( Main.netMode == 2 ) {	// is this needed?
				NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, this.npc.whoAmI );
			}
		}


		////////////////

		public void DefeatEffects() {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			int soundSlot = this.mod.GetSoundSlot( SoundType.Custom, "Sounds/Custom/TricksterLaugh" );

			Main.PlaySound( (int)SoundType.Custom, (int)this.npc.Center.X, (int)this.npc.Center.Y, soundSlot );
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 72, this.npc.width, this.npc.height );

			if( this.npc.target == Main.myPlayer ) {
				switch( rand.Next(6) ) {
				case 0:
					Main.NewText( "I'll be back!", Color.DarkMagenta );
					break;
				case 1:
					Main.NewText( "Ta ta!", Color.DarkMagenta );
					break;
				case 2:
					Main.NewText( "This isn't over!", Color.DarkMagenta );
					break;
				case 3:
					Main.NewText( "Hahahaha!", Color.DarkMagenta );
					break;
				case 4:
					Main.NewText( "Nice try!", Color.DarkMagenta );
					break;
				case 5:
					Main.NewText( "Whoops!", Color.DarkMagenta );
					break;
				}
			}

			if( Main.netMode != 1 ) {
				int itemType = TheTricksterConfig.Instance.DropsOnDefeat?.Type ?? -1;
				if( itemType != -1 ) {
					ItemHelpers.CreateItem( this.npc.position, itemType, 1, 24, 24 );
				}
			}

			//Main.npc[this.npc.whoAmI] = new NPC();
			//this.npc.active = false;

			var myworld = ModContent.GetInstance<TheTricksterWorld>();
			myworld.AddTricksterDefeat();
		}
	}
}