using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.FX;
using HamstarHelpers.Helpers.Items;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Helpers.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TheTrickster.Buffs;


namespace TheTrickster.NPCs {
	partial class TricksterNPC : ModNPC {
		public void LaunchAttack() {
			this.CreateLaunchedAttackFX( TricksterNPC.AttackRadius );

			int radiusSqr = TricksterNPC.AttackRadius * TricksterNPC.AttackRadius;
			int invulnBuffType = ModContent.BuffType<DegreelessnessBuff>();

			for( int i=0; i<Main.npc.Length; i++ ) {
				NPC otherNpc = Main.npc[i];
				if( otherNpc == null || !otherNpc.active || otherNpc.friendly || otherNpc.immortal || otherNpc.whoAmI == this.npc.whoAmI ) {
					continue;
				}

				if( Vector2.DistanceSquared(otherNpc.Center, this.npc.Center) < radiusSqr ) {
					otherNpc.AddBuff( invulnBuffType, TricksterNPC.InvulnTickDuration );
				}
			}
		}


		////////////////

		public void Dodge( int dodgeRadius ) {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();
			int minDist = 18 * 16;
			int minDistSqr = minDist * minDist;

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
				dir *= minDist + ( rand.NextFloat() * (dodgeRadius - minDist) );

				testPos = player.Center + dir;
				dodgeRadius += 1;

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
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 16, this.npc.width, this.npc.height );

			this.npc.position = groundPos - new Vector2( 0, this.npc.height + 1 );
			this.npc.netUpdate = true;

			// After
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 16, this.npc.width, this.npc.height );
		}


		////////////////

		public void Flee() {
			UnifiedRandom rand = TmlHelpers.SafelyGetRand();

			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 32, this.npc.width, this.npc.height );

			for( int i=0; i<3; i++ ) {
				var dir = new Vector2( (rand.NextFloat() - 0.5f), (rand.NextFloat() - 0.5f) );
				Projectile.NewProjectile( this.npc.position, dir * 4f, ProjectileID.HappyBomb, 9999, 40f );
			}

			Main.npc[this.npc.whoAmI] = new NPC();
			this.npc.active = false;
		}


		////////////////

		public void Defeat() {
			int soundSlot = this.mod.GetSoundSlot( SoundType.Custom, "Sounds/Custom/TricksterLaugh" );

			Main.PlaySound( (int)SoundType.Custom, (int)this.npc.Center.X, (int)this.npc.Center.Y, soundSlot );
			ParticleFxHelpers.MakeTeleportFx( this.npc.position, 32, this.npc.width, this.npc.height );

			if( Main.netMode != 1 ) {
				int itemType = TheTricksterMod.Config.DropsOnDefeat?.Type ?? -1;
				if( itemType != -1 ) {
					ItemHelpers.CreateItem( this.npc.position, itemType, 1, 24, 24 );
				}
			}

			//Main.npc[this.npc.whoAmI] = new NPC();
			//this.npc.active = false;
		}
	}
}
