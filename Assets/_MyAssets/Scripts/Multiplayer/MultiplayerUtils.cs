/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Photon;
using ExitGames.Client.Photon;

namespace VOX{
	public static class MultiplayerUtils{
		
		public static void CreateRoom(string roomName, int gamemodeID = 0, int mapID = 0, byte maxPlayers = 16, double roundTime = 300d){
			RoomOptions options = new RoomOptions();
			options.MaxPlayers = maxPlayers;

			string map;
			switch(mapID){
				default:
					map = "House";
					break;
				case 1:
					map = "DevMap";
					break;
				case 2:
					map = "DevMap_Night";
					break;
			}

			options.CustomRoomProperties = new Hashtable(){
				{"gamemode", gamemodeID},
				{"map", map},
				{"timePerRound", roundTime}
			};
			options.CustomRoomPropertiesForLobby = new string[]{"gamemode", "map"};

			PhotonNetwork.CreateRoom(roomName, options, null);
		}

	}
}