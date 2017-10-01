/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VOX{
	public class OptionsWindow : MonoBehaviour{
		
		public GameObject window;

		[Header("Video")]
		public OptionsLevels screenMode;
		public OptionsLevels resolution;
		public OptionsLevels shadows;
		public OptionsLevels antialiasing;

		[Header("Audio")]
		public Slider masterVolumeSlider;
		public Slider sfxVolumeSlider;
		public Slider uiVolumeSlider;

		[Header("Mouse")]
		public TMP_InputField sensitivityField;

		void Awake(){
			Settings.Load();

			shadows.onChangeLevel += ApplyShadows;
			antialiasing.onChangeLevel += ApplyAntialiasing;

			masterVolumeSlider.onValueChanged.AddListener(ApplyMasterVolume);
			sfxVolumeSlider.onValueChanged.AddListener(ApplySFXVolume);
			uiVolumeSlider.onValueChanged.AddListener(ApplyUIVolume);

			sensitivityField.onEndEdit.AddListener(ApplySensitivity);
		}

		void Start(){
			Resolution[] resolutions = Screen.resolutions;
			resolution.levels = new string[resolutions.Length];

			for(int i = 0; i < resolutions.Length; i++){
				Resolution res = resolutions[i];
				resolution.levels[i] = string.Format("{0}x{1}", res.width, res.height);
			}
		}

		public void Open(){
			if(window.activeInHierarchy)
				return;

			screenMode.SetLevel(Settings.ScreenMode);
			resolution.SetLevel(Settings.Resolution);
			shadows.SetLevel(Settings.Shadows);
			antialiasing.SetLevel(Settings.Antialiasing);

			masterVolumeSlider.value = Settings.MasterVolume;
			sfxVolumeSlider.value = Settings.SFXVolume;
			uiVolumeSlider.value = Settings.UIVolume;

			sensitivityField.text = Settings.Sensitivity.ToString();

			window.SetActive(true);
		}

		public void Close(bool apply = true){
			if(!window.activeInHierarchy)
				return;

			window.SetActive(false);

			if(apply){
				Apply(false);
			}
		}

		public void ApplyShadows(int level){
			Settings.UpdateValue(Settings.Setting.Shadows, level);
		}

		public void ApplyAntialiasing(int level){
			Settings.UpdateValue(Settings.Setting.Antialiasing, level);
		}

		public void ApplySensitivity(string sensitivityString = ""){
			float sensitivity = Settings.Sensitivity;
			if(float.TryParse(sensitivityField.text, out sensitivity)){
				Settings.UpdateValue(Settings.Setting.Sensitivity, sensitivity);
			}
		}

		public void ApplyVolumes(){
			ApplyMasterVolume(masterVolumeSlider.value);
			ApplySFXVolume(sfxVolumeSlider.value);
			ApplyUIVolume(uiVolumeSlider.value);
		}

		public void ApplyMasterVolume(float volume){
			Settings.UpdateValue(Settings.Setting.MasterVolume, volume);
		}

		public void ApplySFXVolume(float volume){
			Settings.UpdateValue(Settings.Setting.SFXVolume, volume);
		}

		public void ApplyUIVolume(float volume){
			Settings.UpdateValue(Settings.Setting.UIVolume, volume);
		}

		public void Apply(bool changeResolution = true){
			if(changeResolution){
				bool screenModeUpdate = Settings.UpdateValue(Settings.Setting.ScreenMode, screenMode.currentLevel);
				bool resolutionUpdate = Settings.UpdateValue(Settings.Setting.Resolution, resolution.currentLevel);

				if(screenModeUpdate || resolutionUpdate){
					Settings.UpdateResolution();
				}
			}

			ApplyShadows(shadows.currentLevel);
			ApplyAntialiasing(antialiasing.currentLevel);
			ApplyVolumes();
			ApplySensitivity();

			Settings.Save();
		}
	}
}