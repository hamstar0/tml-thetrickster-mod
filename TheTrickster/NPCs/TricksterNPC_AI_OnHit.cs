using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private bool RunOnHitAI() {
			var config = TheTricksterConfig.Instance;

			switch( this.State ) {
			case TricksterState.Idle:
			case TricksterState.Lurk:
				break;
			case TricksterState.PreAttack:
			case TricksterState.Attack:
				this.SetState( TricksterState.Idle );
				break;
			case TricksterState.Cooldown:
				break;
			}

			int hitsBeforeBlink = config.Get<int>( nameof(TheTricksterConfig.HitsBeforeBlink) );
			bool tooManyHits = this.HitsDuringCurrentStage++ >= hitsBeforeBlink;

			if( tooManyHits ) {
				int minDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MinDodgeRadius) );
				int maxDodgeRad = config.Get<int>( nameof(TheTricksterConfig.MaxDodgeRadius) );

				this.DeployDefenseBats();
				this.DodgeAction( minDodgeRad, maxDodgeRad );

				this.SetState( TricksterState.Idle );
			} else {
				this.ElapsedStateTicks = 1;
			}

			return tooManyHits;
		}
	}
}
