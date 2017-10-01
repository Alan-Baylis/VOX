/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VOX{
	public class ServerUI : MonoBehaviour{
		
		public TextMeshProUGUI serverName;
		public TextMeshProUGUI info;

		public RoomInfo room;

		public void Setup(RoomInfo room){
			this.room = room;
			serverName.text = room.Name;

			ServerController.Gamemode gamemode = (ServerController.Gamemode) room.CustomProperties["gamemode"];
			info.text = string.Format("{0}, {1}/{2}", gamemode.ToString(), room.PlayerCount, room.MaxPlayers);
		}

		public void UI_Join(){
			PhotonNetwork.JoinRoom(room.Name);
		}

	}
}