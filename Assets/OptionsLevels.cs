/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VOX{
	public class OptionsLevels : MonoBehaviour{
		
		public int currentLevel;
		public string[] levels;

		[Header("UI")]
		public TextMeshProUGUI levelText;

		public System.Action<int> onChangeLevel;

		public void SetLevel(int newLevel){
			currentLevel = Mathf.Clamp(newLevel, 0, levels.Length - 1);
			levelText.text = levels[currentLevel];

			if(onChangeLevel != null){
				onChangeLevel(currentLevel);
			}
		}

		public void GoBack(bool resetIfOverLimit){
			if(levels.Length < 2)
				return;

			int newLevel = currentLevel - 1;

			if(currentLevel < 0){
				if(resetIfOverLimit)
					newLevel = levels.Length - 1;
				else
					newLevel = 0;
			}
			
			SetLevel(newLevel);
		}

		public void GoForwards(bool resetIfOverLimit){
			if(levels.Length < 2)
				return;
			

			int newLevel = currentLevel + 1;
			
			if(resetIfOverLimit && currentLevel > levels.Length - 1){
				if(resetIfOverLimit)
					newLevel = 0;
				else
					newLevel = levels.Length - 1;
			}
			
			SetLevel(newLevel);
		}
	}
}