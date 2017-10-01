/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;

namespace VOX{
	public class Tabs : MonoBehaviour{
		
		public Tab[] tabs;

		void Start(){
			SelectTab(0);
		}

		public void SelectTab(int index){
			index = Mathf.Clamp(index, 0, tabs.Length - 1);

			for(int i = 0; i < tabs.Length; i++){
				Tab tab = tabs[i];

				tab.button.interactable = i != index;
				tab.content.SetActive(i == index);
			}
		}
	}

	[System.Serializable]
	public struct Tab{
		public string name;
		public Button button;
		public GameObject content;
	}
}