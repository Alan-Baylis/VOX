/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	[CreateAssetMenu()]
	public class CustomizationOptions : ScriptableObject{
		
		public Texture2D defaultTexture;
		
		[Header("Default Colors")]
		public Color skinColor = Color.white;
		public Color eyesColor = Color.white;
		public Color shirtColor = Color.white;
		public Color pantsColor = Color.white;
		public Color shoesColor = Color.white;

		[Header("Options")]
		public Option[] skinOptions;
		public Option[] eyesOptions;
		public Option[] shirtOptions;
		public Option[] pantsOptions;
		public Option[] shoesOptions;

	}

	[System.Serializable]
	public struct Option{
		public string name;
		public Color color;
	}
}