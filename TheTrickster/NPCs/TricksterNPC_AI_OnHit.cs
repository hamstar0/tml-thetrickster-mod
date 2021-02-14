using System;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private void RunOnHitAI() {

			switch( this.State ) {
			case TricksterState.Mock:
				this.RunOnHitAI_Leave();
				break;
			case TricksterState.Lurk:
				this.RunOnHitAI_DodgeIf( true );
				break;
			case TricksterState.Idle:
				this.RunOnHitAI_DodgeIf( false );
				break;
			case TricksterState.PreAttack:
			case TricksterState.Attack:
				this.SetState( TricksterState.Idle );
				this.RunOnHitAI_DodgeIf( false );
				break;
			case TricksterState.Cooldown:
				this.RunOnHitAI_DodgeIf( false );
				break;
			}
		}


		////

		private void RunOnHitAI_Leave() {
			this.EncounterFormal( true );

			this.FleeAction();
		}

		private void RunOnHitAI_DodgeIf( bool isFormalEncounter ) {
			if( isFormalEncounter ) {
				this.EncounterFormal( false );
			}

			var config = TheTricksterConfig.Instance;
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
		}
	}
}
