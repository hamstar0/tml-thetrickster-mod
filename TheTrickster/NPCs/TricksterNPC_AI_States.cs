using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Collisions;
using TheTrickster.Protocols;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		public int GetCurrentStateTickDuration() {
			switch( this.State ) {
			default:
			case TricksterState.Idle:
				return TheTricksterConfig.Instance.IdleDurationTicks;
			case TricksterState.Lurk:
				return TheTricksterConfig.Instance.LurkDurationTicks;
			case TricksterState.PreAttack:
				return TheTricksterConfig.Instance.IdleDurationTicks;
			case TricksterState.Attack:
				var myworld = ModContent.GetInstance<TheTricksterWorld>();
				int reducedChargeTime = myworld.TricksterDefeats * TheTricksterConfig.Instance.AttackDurationTicksReducedPerDefeat;
				int chargeTime = TheTricksterConfig.Instance.AttackDurationTicks - reducedChargeTime;
				
				return Math.Max( chargeTime, TheTricksterConfig.Instance.AttackDurationTicksMinimum );
			case TricksterState.Cooldown:
				return TheTricksterConfig.Instance.CooldownDurationTicks;
			}
		}

		////////////////

		public void SetState( TricksterState newState, bool syncs=true ) {
			this.State = newState;

			this.HitsDuringCurrentStage = 0;
			this.ElapsedStateTicks = 0;

			switch( newState ) {
			case TricksterState.PreAttack:
				this.OnSetStateForPreAttack();
				break;
			}

			if( syncs && Main.netMode == 2 ) {
				TricksterStateProtocol.Broadcast( this.npc.whoAmI, newState );
			}
		}

		////

		private void OnSetStateForPreAttack() {
			Player plr = this.TargetPlayer;
			if( plr == null ) {
				return;
			}

			(int x, int y) plrTile = ((int)plr.Center.X >> 4, (int)plr.Center.Y >> 4);
			(int x, int y) npcTile = ((int)this.npc.Center.X >> 4, (int)this.npc.Center.Y >> 4);
			IList<(int, int)> path;

			if( TileCollisionHelpers.FindPathSimple( plrTile, npcTile, 2000, out path ) ) {
				this.SetState( TricksterState.Attack, false );
			}
		}
	}
}
