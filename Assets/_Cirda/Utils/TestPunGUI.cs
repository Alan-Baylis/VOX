/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;
using Photon;

namespace Cirda.Utils{
	public class TestPunGUI : PunBehaviour{

		public string gameName;
		public string gameVersion;

		[Header("Spawning")]
		public string playerPrefabName;
		public Vector3 playerSpawnPos;
		public Vector3 playerSpawnRot;

		[Header("Others")]
		public GameObject hudPrefab;
		public GameObject offlineCamera;

		public static GameObject myPlayer;

		int menu = 0;
		string username;
		string hostRoomName;

		RoomInfo[] rooms;

		void Start(){
			PhotonNetwork.ConnectUsingSettings(gameName + "_" + gameVersion);
			rooms = PhotonNetwork.GetRoomList();
			
			username = PlayerPrefs.GetString("TestPunGUI.Username", "Guest");
			hostRoomName = PlayerPrefs.GetString("TestPunGUI.HostRoomName", "Room");
		}

		void OnGUI(){
			if(PhotonNetwork.connected && PhotonNetwork.room == null){
				if(GUI.Button(new Rect(15, Screen.height - 20 - 15, 200, 20), "Quick Room"))
					PhotonNetwork.JoinOrCreateRoom("QuickRoom", new RoomOptions(){ IsVisible = false }, TypedLobby.Default);

				if(menu == 0){
					GUI.Box(new Rect(10, 10, 210, 98), gameName + " - " + gameVersion);

					if(GUI.Button(new Rect(15, 32, 200, 20), "Host Server"))
						menu = 1;
					if(GUI.Button(new Rect(15, 57, 200, 20), "Join Server"))
						menu = 2;

					username = GUI.TextField(new Rect(15, 82, 200, 20), username);
				}else if(menu == 1){
					GUI.Box(new Rect(10, 10, 210, 98), "Host Server");

					hostRoomName = GUI.TextField(new Rect(15, 32, 200, 20), hostRoomName);

					if(GUI.Button(new Rect(15, 57, 200, 20), "Host"))
						PhotonNetwork.CreateRoom(hostRoomName);
					if(GUI.Button(new Rect(15, 82, 200, 20), "Back"))
						menu = 0;
				
				}else if(menu == 2){
					GUI.Box(new Rect(10, 10, 210, 98), "Join Server");

					if(GUI.Button(new Rect(15, 32, 200, 20), "Join Random"))
						PhotonNetwork.JoinRandomRoom();
					if(GUI.Button(new Rect(15, 57, 200, 20), "Refresh"))
						rooms = PhotonNetwork.GetRoomList();
					if(GUI.Button(new Rect(15, 82, 200, 20), "Back"))
						menu = 0;

					GUI.Box(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), "Server List");
					if(rooms != null && rooms.Length > 0){
						for(int i = 0; i < PhotonNetwork.countOfRooms; i++){
							if(GUI.Button(new Rect(Screen.width - 255, 32 + (i * 25), 240, 20), rooms[i].Name))
								PhotonNetwork.JoinRoom(rooms[i].Name);
						}
					}
				}
			}
		}

		public override void OnJoinedRoom(){
			PlayerPrefs.SetString("TestPunGUI.Username", username);
			PlayerPrefs.SetString("TestPunGUI.HostRoomName", hostRoomName);

			PhotonNetwork.playerName = username;

			if(hudPrefab != null)
				Instantiate(hudPrefab);

			if(playerPrefabName != "")
				myPlayer = PhotonNetwork.Instantiate(playerPrefabName, playerSpawnPos, Quaternion.Euler(playerSpawnRot), 0);

			if(offlineCamera != null)
				offlineCamera.SetActive(false);
		}
	}
}