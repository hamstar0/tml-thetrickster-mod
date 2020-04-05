using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using HamstarHelpers.Buffs;
using HamstarHelpers.Classes.Tiles.TilePattern;
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
				if( otherNpc == null || !otherNpc.active || otherNpc.friendly || otherNpc.immortal || otherNpc.whoAmI == this.npc.whoAmI ) {
					continue;
				}

				if( Vector2.DistanceSquared(otherNpc.Center, this.npc.Center) < radiusSqr ) {
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
				int bats = 2 + Main.rand.Next( 3 );

				for( int i=0; i<bats; i++ ) {
					this.DeployDefenseBat();
				}
			}
		}

		private void DeployDefenseBat() {
			int npcWho = NPC.NewNPC( (int)this.npc.position.X, (int)this.npc.position.Y, NPCID.CaveBat, 0, 0f, 0f, 0f, 0f, this.npc.target );
			NPC npc = Main.npc[npcWho];
			if( !npc.active ) {
				return;
			}

			npc.scale = 0.5f;
			npc.life = 2;
			npc.defense = 999999;
			npc.color *= 0.5f;

			var mynpc = npc.GetGlobalNPC<TheTricksterGlobalNPC>();
			mynpc.IsTricksterBat = true;

			if( Main.netMode == 2 ) {
				NetMessage.SendData( MessageID.SyncNPC, -1, -1, null, this.npc.whoAmI );
				TricksterBatProtocol.Broadcast( npcWho );
			}
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

		private int AttackChargeSideEffectCooldown = 0;

		public void AttackChargeSideBehaviors() {
			if( this.AttackChargeSideEffectCooldown-- > 0 ) {
				return;
			}
			this.AttackChargeSideEffectCooldown = 10;

			float attackRangeSqr = TheTricksterConfig.Instance.AttackRadius * TheTricksterConfig.Instance.AttackRadius;
			int maxProjs = Main.projectile.Length;
			int maxPlrs = Main.player.Length;

			for( int i=0; i<maxProjs; i++ ) {
				Projectile proj = Main.projectile[i];
				if( proj?.active != true ) {
					continue;
				}

				if( (proj.Center - this.npc.Center).LengthSquared() < attackRangeSqr ) {
					proj.velocity *= 0.95f;
				}
			}

			for( int i=0; i<maxPlrs; i++ ) {
				Player plr = Main.player[i];
				if( plr?.active != true ) {
					continue;
				}

				if( (plr.Center - this.npc.Center).LengthSquared() < attackRangeSqr ) {
					plr.velocity *= 0.95f;
				}
			}
		}


		public void DefeatSideBehaviors() {
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
