﻿using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using HamstarHelpers.Classes.DataStructures;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Maps;


namespace TheTrickster {
	class TheTricksterWorld : ModWorld {
		private ISet<(int, int)> _TricksterDefeatLocations;


		////////////////

		public ISet<(int, int)> TricksterDefeatLocations { get; private set; }



		////////////////

		public TheTricksterWorld() : base() {
			this._TricksterDefeatLocations = new HashSet<(int, int)>();
			this.TricksterDefeatLocations = new ReadOnlySet<(int, int)>( this._TricksterDefeatLocations );
		}


		////////////////

		public override void Initialize() {
			//this._TricksterDefeatLocations.Clear();
		}

		////

		public override void Load( TagCompound tag ) {
			this._TricksterDefeatLocations.Clear();

			if( tag.ContainsKey("trickster_defeats") ) {
				int defeats = tag.GetInt( "trickster_defeats" );

				for( int i=0; i<defeats; i++ ) {
					int x = tag.GetInt( "trickster_defeat_x_" + i );
					int y = tag.GetInt( "trickster_defeat_y_" + i );

					this._TricksterDefeatLocations.Add( (x, y) );

					this.AddTricksterDefeatToMap( x, y );
					//LogHelpers.Log( "Loaded "+(i+1)+" (of "+defeats+") victories over Trickster ("+x+", "+y+")." );
				}
			}
		}

		public override TagCompound Save() {
			var tag = new TagCompound {
				{ "trickster_defeats", this._TricksterDefeatLocations.Count }
			};

			int i = 0;
			foreach( (int tileX, int tileY) in this._TricksterDefeatLocations ) {
				tag["trickster_defeat_x_"+i] = tileX;
				tag["trickster_defeat_y_"+i] = tileY;
				i++;
			}

			LogHelpers.Log( "Saved "+this._TricksterDefeatLocations.Count+" victories over Trickster." );

			return tag;
		}


		////////////////

		public override void NetReceive( BinaryReader reader ) {
			if( reader == null ) { return; }
			try {
				int defeats = reader.ReadInt32();

				this._TricksterDefeatLocations.Clear();

				for( int i = 0; i < defeats; i++ ) {
					int x = reader.ReadInt32();
					int y = reader.ReadInt32();

					this._TricksterDefeatLocations.Add( (x, y) );
				}
			} catch { }
		}

		public override void NetSend( BinaryWriter writer ) {
			if( writer == null ) { return; }
			try {
				writer.Write( (int)this._TricksterDefeatLocations.Count );

				foreach( (int tileX, int tileY) in this._TricksterDefeatLocations ) {
					writer.Write( (int)tileX );
					writer.Write( (int)tileY );
				}
			} catch { }
		}


		////////////////

		public void AddTricksterDefeat( NPC tricksterNpc ) {
			int tileX = (int)( tricksterNpc.position.X / 16f );
			int tileY = (int)( tricksterNpc.position.Y / 16f );

			this._TricksterDefeatLocations.Add( (tileX, tileY) );

			this.AddTricksterDefeatToMap( tileX, tileY );
		}

		////

		private void AddTricksterDefeatToMap( int tileX, int tileY ) {
			if( Main.netMode != NetmodeID.Server ) {
				MapMarkers.SetFullScreenMapMarker(
					id: "Trickster Defeat " + this.TricksterDefeatLocations.Count,
					tileX: tileX,
					tileY: tileY,
					icon: TheTricksterMod.Instance.MapIcon,
					scale: 1.5f
				);
			}
		}
	}
}
