using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Services.Maps;
using Terraria.ID;

namespace TheTrickster {
	class TheTricksterWorld : ModWorld {
		private IDictionary<int, int> _TricksterDefeatLocations = new ConcurrentDictionary<int, int>();


		////////////////

		public int TricksterDefeats { get; private set; } = 0;

		public IReadOnlyDictionary<int, int> TricksterDefeatLocations { get; private set; }



		////////////////

		public TheTricksterWorld() : base() {
			this.TricksterDefeatLocations = new ReadOnlyDictionary<int, int>( this._TricksterDefeatLocations );
		}


		////////////////

		public override void Initialize() {
			this._TricksterDefeatLocations.Clear();
			this.TricksterDefeats = 0;
		}

		////

		public override void Load( TagCompound tag ) {
			this._TricksterDefeatLocations.Clear();

			if( tag.ContainsKey("trickster_defeats") ) {
				this.TricksterDefeats = tag.GetInt( "trickster_defeats" );
			}
			if( tag.ContainsKey("trickster_defeat_x_0") ) {
				for( int i=0; i<this.TricksterDefeats; i++ ) {
					int x = tag.GetInt( "trickster_defeat_x_" + i );
					int y = tag.GetInt( "trickster_defeat_y_" + i );
					this._TricksterDefeatLocations[ x ] = y;
				}
			}
		}

		public override TagCompound Save() {
			var tag = new TagCompound {
				{ "trickster_defeats", this.TricksterDefeats }
			};

			int i = 0;
			foreach( (int tileX, int tileY) in this._TricksterDefeatLocations ) {
				tag["trickster_defeat_x_"+i] = tileX;
				tag["trickster_defeat_y_"+i] = tileY;
				i++;
			}

			return tag;
		}


		////////////////

		public override void NetReceive( BinaryReader reader ) {
			if( reader == null ) { return; }
			try {
				this.TricksterDefeats = reader.ReadInt32();

				this._TricksterDefeatLocations.Clear();

				for( int i = 0; i < this.TricksterDefeats; i++ ) {
					int x = reader.ReadInt32();
					int y = reader.ReadInt32();

					this._TricksterDefeatLocations[x] = y;
				}
			} catch { }
		}

		public override void NetSend( BinaryWriter writer ) {
			if( writer == null ) { return; }
			try {
				writer.Write( (int)this.TricksterDefeats );

				int i = 0;
				foreach( (int tileX, int tileY) in this._TricksterDefeatLocations ) {
					writer.Write( (int)tileX );
					writer.Write( (int)tileY );
					i++;
				}
			} catch { }
		}


		////////////////

		public void AddTricksterDefeat( NPC tricksterNpc ) {
			this.TricksterDefeats += 1;

			int tileX = (int)( tricksterNpc.position.X / 16f );
			int tileY = (int)( tricksterNpc.position.Y / 16f );

			this._TricksterDefeatLocations[ tileX ] = tileY;

			if( Main.netMode != NetmodeID.Server ) {
				MapMarkers.AddFullScreenMapMarker(
					tileX,
					tileY,
					"Trickster Defeat " + this.TricksterDefeats,
					TheTricksterMod.Instance.MapIcon
				);
			}
		}
	}
}
