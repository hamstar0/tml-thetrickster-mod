using System;
using Terraria;
using Terraria.ModLoader;


namespace TheTrickster.NPCs {
	public partial class TricksterNPC : ModNPC {
		private TricksterDecision RunOnHitAI() {
			switch( this.State ) {
			case TricksterState.Mock:
				return this.RunOnHitAI_Leave( false );
			case TricksterState.Lurk:
				return this.RunOnHitAI_DodgeIf( true );
			case TricksterState.Idle:
				return this.RunOnHitAI_DodgeIf( false );
			case TricksterState.PreAttack:
			case TricksterState.AttackChargeup:
				this.SetState( TricksterState.Idle );
				return this.RunOnHitAI_DodgeIf( false );
			case TricksterState.Cooldown:
				return this.RunOnHitAI_DodgeIf( false );
			}
			return TricksterDecision.None;
		}


		////

		private TricksterDecision RunOnHitAI_Leave( bool dropBombs ) {
			this.EncounterFormal( true );

			//this.FleeAction( dropBombs );
			if( dropBombs ) {
				return TricksterDecision.FleeWithBombs;
			} else {
				return TricksterDecision.FleeNoBombs;
			}
		}

		private TricksterDecision RunOnHitAI_DodgeIf( bool isFormalEncounter ) {
			if( isFormalEncounter ) {
				this.EncounterFormal( false );
			}

			var config = TheTricksterConfig.Instance;
			int hitsBeforeBlink = config.Get<int>( nameof(TheTricksterConfig.HitsBeforeBlink) );
			bool tooManyHits = this.HitsDuringCurrentStage++ >= hitsBeforeBlink;

			if( tooManyHits ) {
				int minDodgeRad = config.Get<int>( nameof(config.MinDodgeRadius) );
				int maxDodgeRad = config.Get<int>( nameof(config.MaxDodgeRadius) );

				this.DeployDefenseBats();

				this.DodgeAction( minDodgeRad, maxDodgeRad, true );

				this.SetState( TricksterState.Idle );
			} else {
				this.ElapsedStateTicks = 1;
			}

			return TricksterDecision.None;
		}
	}
}
