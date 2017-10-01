/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.Audio;
using GameJolt.API;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

namespace VOX{
	public class GameController : Singleton<GameController>{
		
		protected GameController(){}

		public LayerMask fireLayers;
		public LayerMask afterKillFireLayers;
		public LayerMask explosionLayers;

		[Header("Audio")]
		public AudioMixer audioMixer;

		public static int skin = 0;
		public static int eyes = 1;
		public static int shirt = 2;
		public static int pants = 1;
		public static int shoes = 1;
		public static byte[] characterTexture;

		public static int totalKills = -1;
		public static int totalDeaths = -1;

		private static GunInfo[] guns;
		
		void Awake(){
			Settings.onLoad += OnLoad;
			Settings.onValueUpdate += OnValueUpdate;

			guns = Resources.LoadAll<GunInfo>("_Guns");

			Array.Sort(guns, delegate(GunInfo a, GunInfo b){
				return a.ID.CompareTo(b.ID);
			});
		}

		public static void LoadGameJoltData(Action<bool> callback = null){
			UpdatePlayerProperties();

			if(Manager.Instance != null && Manager.Instance.CurrentUser != null){
				DataStore.Get("character.skin", false, (string skin) => {
					if(skin != null){
						GameController.skin = int.Parse(skin);
						UpdatePlayerProperties();
					}

					if(callback != null){
						callback(true);
					}
				});
				DataStore.Get("character.eyes", false, (string eyes) => {
					if(eyes != null){
						GameController.eyes = int.Parse(eyes);
						UpdatePlayerProperties();
					}

					if(callback != null){
						callback(true);
					}
				});
				DataStore.Get("character.shirt", false, (string shirt) => {
					if(shirt != null){
						GameController.shirt = int.Parse(shirt);
						UpdatePlayerProperties();

						if(callback != null){
							callback(true);
						}
					}

					if(callback != null){
						callback(true);
					}
				});
				DataStore.Get("character.pants", false, (string pants) => {
					if(pants != null){
						GameController.pants = int.Parse(pants);
						UpdatePlayerProperties();
					}

					if(callback != null){
						callback(true);
					}
				});
				DataStore.Get("character.shoes", false, (string shoes) => {
					if(shoes != null){
						GameController.shoes = int.Parse(shoes);
						UpdatePlayerProperties();
					}

					if(callback != null){
						callback(true);
					}
				});
				DataStore.Get("character.texture", false, (string texture) => {
					if(texture != null){
						GameController.characterTexture = System.Convert.FromBase64String(texture);
						UpdatePlayerProperties();
					}

					if(callback != null){
						callback(true);
					}
				});
				DataStore.Get("totalKills", false, (string totalKillsStr) => {
					if(totalKillsStr != null){
						int totalKillsInt = 0;
						if(int.TryParse(totalKillsStr, out totalKillsInt)){
							totalKills = totalKillsInt;
						}
					}else{
						totalKills = 0;
					}

					if(callback != null){
						callback(true);
					}
				});
				DataStore.Get("totalDeaths", false, (string totalDeathsStr) => {
					if(totalDeathsStr != null){
						int totalDeathsInt = 0;
						if(int.TryParse(totalDeathsStr, out totalDeathsInt)){
							totalDeaths = totalDeathsInt;
						}
					}else{
						totalDeaths = 0;
					}

					if(callback != null){
						callback(true);
					}
				});
			}else{
				if(callback != null){
					callback(false);
				}
			}
		}

		public static void UpdatePlayerProperties(){
			Hashtable hash = PhotonNetwork.player.CustomProperties;
			
			hash["character.skin"] = skin;
			hash["character.eyes"] = eyes;
			hash["character.shirt"] = shirt;
			hash["character.pants"] = pants;
			hash["character.shoes"] = shoes;
			hash["character.texture"] = characterTexture;
			hash["avatarURL"] = Manager.Instance.CurrentUser.AvatarURL;

			PhotonNetwork.player.SetCustomProperties(hash);
		}

		public static GunInfo[] GetAllGuns(){
			return guns;
		}

		public static GunInfo GetGun(int ID){
			return guns[Mathf.Clamp(ID, 0, guns.Length - 1)];
		}

		public static bool IsPaused(){
			Player player = ServerController.player;

			if(player != null && MapController.hud != null){
				if(player.photonView.isMine){
					return (
						MapController.hud.menu.activeInHierarchy ||
						MapController.hud.chatField.gameObject.activeInHierarchy ||
						MapController.hud.classCreation.classes.activeInHierarchy ||
						ServerController.HasRoundEnded
					);
				}
			}

			return false;
		}

		public void OnLoad(){
			audioMixer.SetFloat("MasterVolume", LinearToDecibel(Settings.MasterVolume));
			audioMixer.SetFloat("SFXVolume", LinearToDecibel(Settings.SFXVolume));
			audioMixer.SetFloat("UIVolume", LinearToDecibel(Settings.UIVolume));

			if(Settings.Antialiasing == 0){
				QualitySettings.antiAliasing = 0;
			}else if(Settings.Antialiasing == 1){
				QualitySettings.antiAliasing = 2;
			}else if(Settings.Antialiasing == 2){
				QualitySettings.antiAliasing = 4;
			}else if(Settings.Antialiasing == 3){
				QualitySettings.antiAliasing = 8;
			}
		}

		public void OnValueUpdate(Settings.Setting setting, object value){
			if(setting == Settings.Setting.MasterVolume || setting == Settings.Setting.SFXVolume ||
				setting == Settings.Setting.UIVolume || setting == Settings.Setting.Antialiasing
			){
				OnLoad();
			}
		}


		
		public static float LinearToDecibel(float linear){
             if(linear > 0f)
                 return 20f * Mathf.Log10(linear);

			return -144f;
         }
	}
}