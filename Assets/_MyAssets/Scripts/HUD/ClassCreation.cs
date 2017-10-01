/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace VOX{
	public class ClassCreation : MonoBehaviour{
		
		public enum SelectMenuType{ Primary = 0, Secondary = 1, Melee = 2, Item1 = 3, Item2 = 4 }

		public GameObject classes;
		public GameObject main;
		public ScrollRect scrollRect;

		[Header("Team Selection")]
		public GameObject teamSelectMenu;

		[Header("Select Menu")]
		public Transform classItemParent;
		public GameObject classItemPrefab;
		public GameObject selectMenu;

		[Header("Selected Weapons")]
		public GunInfo primary;
		public GunInfo secondary;
		public GunInfo melee;
		public GunInfo item1;
		public GunInfo item2;

		[Header("Selections")]
		public ClassItem primaryItem;
		public ClassItem secondaryItem;
		public ClassItem meleeItem;
		public ClassItem item1Item;
		public ClassItem item2Item;
		public ClassItem selectionItem;

		GunInfo currentSelected;
		SelectMenuType currentSelectMenu;

		ClassItem[] items;

		void Start(){
			UpdateItems();
			OnInitializeGamemode();

			GunInfo[] allGuns = GameController.GetAllGuns();
			items = new ClassItem[allGuns.Length];

			for(int i = 0; i < allGuns.Length; i++){
				GunInfo gun = allGuns[i];

				GameObject go = Instantiate(classItemPrefab) as GameObject;
				go.transform.SetParent(classItemParent);
				go.transform.localScale = Vector3.one;

				ClassItem classItem = go.GetComponent<ClassItem>();
				classItem.Refresh(gun);
				classItem.classCreation = this;

				items[i] = classItem;
			}
		}

		public void OnInitializeGamemode(){
			PhotonPlayer player = PhotonNetwork.player;
			ServerController.Gamemode gamemode = ServerController.gamemode;

			if(gamemode == ServerController.Gamemode.FFA){
				player.SetTeam(PunTeams.Team.none);
				Open();
			}else if(gamemode == ServerController.Gamemode.TDM){
				Open(false, true);
			}else if(gamemode == ServerController.Gamemode.OITC){
				player.SetTeam(PunTeams.Team.none);
				SpawnPlayer(-1, 1, 8, -1, -1);
			}else if(gamemode == ServerController.Gamemode.GG){
				player.SetTeam(PunTeams.Team.none);

				int kills = ServerController.GetKills(player);
				GunInfo ggGun = ServerController.Instance.gunGameGuns[kills];

				SpawnPlayer(ggGun.ID, -1, 8, -1, -1);
			}else if(gamemode == ServerController.Gamemode.CTF){
				Open(false, true);
			}
		}

		bool teamOnly;

		public void Open(bool teamOnly = false, bool showTeamSelection = false){
			if(teamOnly)
				showTeamSelection = true;

			this.teamOnly = teamOnly;

			classes.SetActive(true);
			main.SetActive(!showTeamSelection);
			teamSelectMenu.SetActive(showTeamSelection);
			selectMenu.SetActive(false);
		}

		public void Close(){
			classes.SetActive(false);
			main.SetActive(false);
			teamSelectMenu.SetActive(false);
			selectMenu.SetActive(false);
		}

		public void UI_OpenSelectMenu(int typeID){
			SelectMenuType type = (SelectMenuType) Mathf.Clamp(typeID, 0, 4);
			GunInfo.GunType gunType = (GunInfo.GunType) Mathf.Clamp(typeID, 0, 3);

			currentSelectMenu = type;

			if(type == SelectMenuType.Primary)
				currentSelected = primary;
			else if(type == SelectMenuType.Secondary)
				currentSelected = secondary;
			else if(type == SelectMenuType.Melee)
				currentSelected = melee;
			else if(type == SelectMenuType.Item1)
				currentSelected = item1;
			else
				currentSelected = item2;

			for(int i = 0; i < items.Length; i++){
				ClassItem classItem = items[i];
				classItem.gameObject.SetActive(classItem.info.type == gunType);
			}

			scrollRect.verticalNormalizedPosition = 1f;
			UpdateItems();

			main.SetActive(false);
			selectMenu.SetActive(true);
		}

		public void UI_CloseSelectMenu(){ 
			UpdateItems();

			main.SetActive(true);
			selectMenu.SetActive(false);
		}

		public void UI_FinalSelect(){
			if(currentSelectMenu == SelectMenuType.Primary){
				primary = currentSelected;
			}else if(currentSelectMenu == SelectMenuType.Secondary){
				secondary = currentSelected;
			}else if(currentSelectMenu == SelectMenuType.Melee){
				melee = currentSelected;
			}else if(currentSelectMenu == SelectMenuType.Item1){
				item1 = currentSelected;
			}else{
				item2 = currentSelected;
			}

			UI_CloseSelectMenu();
		}

		public void UI_SelectTeam(bool red){
			PhotonNetwork.player.SetTeam(red ? PunTeams.Team.red : PunTeams.Team.blue);

			if(!teamOnly){
				teamSelectMenu.SetActive(false);
				main.SetActive(true);
			}else{
				UI_Start();
			}
		}

		public void UI_Start(){
			if(ServerController.player == null){
				SpawnPlayer(primary.ID, secondary.ID, melee.ID, item1.ID, item2.ID);
			}else{
				ServerController.player.Respawn(primary.ID, secondary.ID, melee.ID, item1.ID, item2.ID);
			}

			Close();
		}

		public void SpawnPlayer(int primaryID, int secondaryID, int meleeID, int item1ID, int item2ID){
			object[] data = new object[5];
			data[0] = primaryID;
			data[1] = secondaryID;
			data[2] = meleeID;
			data[3] = item1ID;
			data[4] = item2ID;
				
			Spawnpoint spawnpoint = MapController.FindSpawnpoint(PhotonNetwork.player.GetTeam());
			PhotonNetwork.Instantiate("Player", spawnpoint.position, spawnpoint.rotation, 0, data);
		}

		public void Select(GunInfo info){
			currentSelected = info;
			UpdateItems();
		}

		public void UpdateItems(){
			primaryItem.Refresh(primary);
			secondaryItem.Refresh(secondary);
			meleeItem.Refresh(melee);
			item1Item.Refresh(item1);
			item2Item.Refresh(item2);
			selectionItem.Refresh(currentSelected);
		}
	}
}