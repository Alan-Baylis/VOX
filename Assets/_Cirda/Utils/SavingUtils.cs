using UnityEngine;

public static class SavingUtils{

	public static class Regedit{
		public static bool HasKey(string key){
			return PlayerPrefs.HasKey(key);
		}

		public static void SetString(string key, string value){
			PlayerPrefs.SetString(key, value);
		}

		public static void SetInt(string key, int value){
			PlayerPrefs.SetInt(key, value);
		}

		public static void SetFloat(string key, float value){
			PlayerPrefs.SetFloat(key, value);
		}

		public static void SetBool(string key, bool value){
			PlayerPrefs.SetInt(key, BoolToInt(value));
		}

		public static string GetString(string key, string defValue){
			if(!HasKey(key))
				SetString(key, defValue);
			return PlayerPrefs.GetString(key);
		}

		public static int GetInt(string key, int defValue){
			if(!HasKey(key))
				SetInt(key, defValue);
			return PlayerPrefs.GetInt(key);
		}

		public static float GetFloat(string key, float defValue){
			if(!HasKey(key))
				SetFloat(key, defValue);
			return PlayerPrefs.GetFloat(key);
		}

		public static bool GetBool(string key, bool defValue){
			if(!HasKey(key))
				SetBool(key, defValue);
			return IntToBool(PlayerPrefs.GetInt(key));
		}
	}

	public static class Strings{
		public const string VFX_SS = "visualEffects.SS";
		public const string VFX_SSAO = "visualEffects.SSAO";
		public const string VFX_DOF = "visualEffects.DOF";
		public const string VFX_MOTIONBLUR = "visualEffects.motionBlur";
		public const string VFX_VIGNETTE = "visualEffects.vignette";
		public const string VFX_BLOOM = "visualEffects.bloom";
		public const string VFX_COLORCORRECTION = "visualEffects.colorCorrection";
	}

	public static bool IntToBool(int integer){
		return integer == 1;
	}

	public static int BoolToInt(bool boolean){
		return boolean ? 1 : 0;
	}
}