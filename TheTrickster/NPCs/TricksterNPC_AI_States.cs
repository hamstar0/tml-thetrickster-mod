using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsGeneral.Libraries.Collisions;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public static int GetCurrentStateTickDuration( TricksterState state ) {
			var config = TheTricksterConfig.Instance;

			switch( state ) {
			default:
			case TricksterState.Mock:
				return config.Get<int>( nameof( config.MaxEncounterDurationTicks ) );
			case TricksterState.Idle:
				return config.Get<int>( nameof( config.IdleDurationTicks ) );
			case TricksterState.Lurk:
				return config.Get<int>( nameof( config.LurkDurationTicks ) );
			case TricksterState.PreAttack:
				return config.Get<int>( nameof( config.IdleDurationTicks ) );
			case TricksterState.AttackChargeup:
				var myworld = ModContent.GetInstance<TheTricksterWorld>();
				int attackTicksLessPer = config.Get<int>( nameof( config.AttackChargeupDurationTicksReducedPerDefeat ) );
				int reducedChargeTime = myworld.TricksterDefeatLocations.Count * attackTicksLessPer;
				int attackDuration = config.Get<int>( nameof( config.AttackChargeupDurationTicks ) );
				int chargeTime = attackDuration - reducedChargeTime;

				return Math.Max( chargeTime, config.Get<int>( nameof(config.AttackChargeupDurationTicksMinimum) ) );
			case TricksterState.Cooldown:
				return config.Get<int>( nameof( config.CooldownDurationTicks ) );
			}
		}



		////////////////

		public void SetState( TricksterState newState, bool syncsIfServer = true, bool forcedForClient=false ) {
			if( !forcedForClient && Main.netMode == NetmodeID.MultiplayerClient ) {
				return;
			}

			this.State = newState;

			this.HitsDuringCurrentStage = 0;
			this.ElapsedStateTicks = 0;

			this.PostSetState( ref newState );

			if( syncsIfServer && Main.netMode == NetmodeID.Server ) {
				TricksterStateProtocol.BroadcastToClients( this.npc.whoAmI, newState );
			}
		}

		////

		private void PostSetState( ref TricksterState state ) {
			Player plr = this.TargetPlayer;
			if( plr == null ) {
				return;
			}

			switch( state ) {
			case TricksterState.PreAttack:
				this.PostSetState_PreAttack( plr, ref state );
				break;
			}
		}


		private void PostSetState_PreAttack( Player player, ref TricksterState state ) {
			(int x, int y) plrTile = ((int)player.Center.X / 16, (int)player.Center.Y / 16);
			(int x, int y) npcTile = ((int)this.npc.Center.X / 16, (int)this.npc.Center.Y / 16);
			IList<(int, int)> path;

			if( TileCollisionLibraries.FindPathSimple( plrTile, npcTile, 2000, out path ) ) {
				state = TricksterState.AttackChargeup;

				this.SetState( state, false );
			}
		}
	}
}
