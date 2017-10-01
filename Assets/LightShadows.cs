/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class LightShadows : MonoBehaviour{
		
		Light light;

		void Awake(){
			light = GetComponent<Light>();

			Settings.onLoad += OnLoad;
			Settings.onValueUpdate += OnValueUpdate;
		}

		void OnEnable(){
			OnLoad();
		}

		public void OnLoad(){
			if(light == null)
				return;

			if(Settings.Shadows == 0){
				//Disabled
				light.shadows = UnityEngine.LightShadows.None;
				QualitySettings.shadowDistance = 0f;
			}else if(Settings.Shadows == 1){
				//Low
				QualitySettings.SetQualityLevel(0);

				light.shadows = UnityEngine.LightShadows.Soft;
				QualitySettings.shadowDistance = 20f;
			}else if(Settings.Shadows == 2){
				//Medium
				QualitySettings.SetQualityLevel(1);

				light.shadows = UnityEngine.LightShadows.Hard;
				QualitySettings.shadowDistance = 100f;
			}else if(Settings.Shadows == 3){
				//High
				QualitySettings.SetQualityLevel(2);

				light.shadows = UnityEngine.LightShadows.Hard;
				QualitySettings.shadowDistance = 150f;
			}else if(Settings.Shadows == 4){
				//Ultra
				QualitySettings.SetQualityLevel(3);

				light.shadows = UnityEngine.LightShadows.Hard;
				QualitySettings.shadowDistance = 200f;
			}
		}

		public void OnValueUpdate(Settings.Setting setting, object value){
			if(setting == Settings.Setting.Shadows){
				OnLoad();
			}
		}
	}
}