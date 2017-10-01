/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using GameJolt.API;
using Photon;
using ExitGames.Client.Photon;
using System;

namespace VOX{
	public static class Settings{

		public enum Setting{
			ScreenMode, Resolution, Shadows, Antialiasing,
			MasterVolume, SFXVolume, UIVolume,
			Sensitivity, LeftHanded, GlobalViewmodelOffset
		}
		
		public static int ScreenMode{ get; private set; }
		public static int Resolution{ get; private set; }
		public static int Shadows{ get; private set; }
		public static int Antialiasing{ get; private set; }

		public static float MasterVolume{ get; private set; }
		public static float SFXVolume{ get; private set; }
		public static float UIVolume{ get; private set; }

		public static float Sensitivity{ get; private set; }
		public static bool LeftHanded{ get; private set; }
		public static Vector3 GlobalViewmodelOffset{ get; private set; }

		public static Action onLoad;
		public static Action onSave;
		public static Action<Setting, object> onValueUpdate;

		public static void Load(){
			ScreenMode = PlayerPrefs.GetInt("settings.screenMode", 1);
			Resolution = PlayerPrefs.GetInt("settings.resolution", Screen.resolutions.Length);
			Shadows = PlayerPrefs.GetInt("settings.shadows", 0);
			Antialiasing = PlayerPrefs.GetInt("settings.antialiasing", 0);
			
			MasterVolume = PlayerPrefs.GetFloat("settings.masterVolume", 1f);
			SFXVolume = PlayerPrefs.GetFloat("settings.sfxVolume", 1f);
			UIVolume = PlayerPrefs.GetFloat("settings.uiVolume", 1f);

			Sensitivity = PlayerPrefs.GetFloat("settings.sensitivity", 1f);
			LeftHanded = GetBool("settings.leftHanded", false);
			GlobalViewmodelOffset = GetVector3FromString(PlayerPrefs.GetString("settings.globalViewmodelOffset", "0,0,0"));

			UpdateResolution();

			if(onLoad != null){
				onLoad();
			}
		}

		public static void Save(){
			PlayerPrefs.SetInt("settings.screenMode", ScreenMode);
			PlayerPrefs.SetInt("settings.resolution", Resolution);
			PlayerPrefs.SetInt("settings.shadows", Shadows);
			PlayerPrefs.SetInt("settings.antialiasing", Antialiasing);

			PlayerPrefs.SetFloat("settings.masterVolume", MasterVolume);
			PlayerPrefs.SetFloat("settings.sfxVolume", SFXVolume);
			PlayerPrefs.SetFloat("settings.uiVolume", UIVolume);

			PlayerPrefs.SetFloat("settings.sensitivity", Sensitivity);
			SetBool("settings.leftHanded", LeftHanded);
			PlayerPrefs.SetString("settings.globalViewmodelOffset", GetStringFromVector3(GlobalViewmodelOffset));

			if(onSave != null){
				onSave();
			}
		}

		public static void UpdateResolution(){
			Resolution[] resolutions = Screen.resolutions;
			Resolution res = resolutions[Mathf.Clamp(Resolution, 0, resolutions.Length - 1)];
			Screen.SetResolution(res.width, res.height, ScreenMode == 1);
		}

		public static bool UpdateValue(Setting setting, object value){
			bool isDifferent = false;

			if(setting == Setting.ScreenMode){
				if(value is int){
					isDifferent = (int) value != ScreenMode;
					ScreenMode = (int) value;
				}
			}else if(setting == Setting.Resolution){
				if(value is int){
					isDifferent = (int) value != Resolution;
					Resolution = (int) value;
				}
			}else if(setting == Setting.Shadows){
				if(value is int){
					isDifferent = (int) value != Shadows;
					Shadows = (int) value;
				}
			}else if(setting == Setting.Antialiasing){
				if(value is int){
					isDifferent = (int) value != Antialiasing;
					Antialiasing = (int) value;
				}
			}else if(setting == Setting.MasterVolume){
				if(value is float){
					isDifferent = (float) value != MasterVolume;
					MasterVolume = (float) value;
				}
			}else if(setting == Setting.SFXVolume){
				if(value is float){
					isDifferent = (float) value != SFXVolume;
					SFXVolume = (float) value;
				}
			}else if(setting == Setting.UIVolume){
				if(value is float){
					isDifferent = (float) value != UIVolume;
					UIVolume = (float) value;
				}
			}else if(setting == Setting.Sensitivity){
				if(value is float){
					isDifferent = (float) value != Sensitivity;
					Sensitivity = (float) value;
				}
			}else if(setting == Setting.LeftHanded){
				if(value is bool){
					isDifferent = (bool) value != LeftHanded;
					LeftHanded = (bool) value;
				}
			}else if(setting == Setting.GlobalViewmodelOffset){
				if(value is Vector3){
					isDifferent = (Vector3) value != GlobalViewmodelOffset;
					GlobalViewmodelOffset = (Vector3) value;
				}
			}
			
			if(isDifferent && onValueUpdate != null){
				onValueUpdate(setting, value);
			}

			return isDifferent;
		}


		
		public static Vector3 GetVector3FromString(string str){
			string[] splitString = str.Split(',');
			return new Vector3(float.Parse(splitString[0]), float.Parse(splitString[1]), float.Parse(splitString[2]));
		}

		public static string GetStringFromVector3(Vector3 vector){
			return string.Format("{0},{1},{2}", vector.x, vector.y, vector.z);
		}

		public static bool GetBool(string key, bool defaultValue){
			int defaultValueInt = defaultValue ? 1 : 0;
			return PlayerPrefs.GetInt(key, defaultValueInt) == 1;
		}

		public static void SetBool(string key, bool value){
			int valueInt = value ? 1 : 0;
			PlayerPrefs.SetInt(key, valueInt);
		}
	}
}