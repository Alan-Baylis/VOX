/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace VOX{
	public class ClassItem : MonoBehaviour, IPointerClickHandler{

		public GunInfo info;
		public ClassCreation classCreation;

		[Header("UI")]
		public Image sprite;
		public TextMeshProUGUI nameText;

		void Start(){
			Refresh();
		}

		public void Refresh(){
			nameText.text = info != null ? info.Name : "";

			if(info != null && info.sprite != null){
				sprite.enabled = true;
				sprite.sprite = info.sprite;
			}else{
				sprite.enabled = false;
				sprite.sprite = null;
			}
		}

		public void Refresh(GunInfo newGunInfo){
			if(newGunInfo != null)
				info = newGunInfo;

			Refresh();
		}

		public void UI_Select(){
			if(info != null)
				classCreation.Select(info);
		}

		public void OnPointerClick(PointerEventData eventData){
			if(eventData.clickCount >= 2){
				UI_Select();
				classCreation.UI_FinalSelect();
			}
		}
	}
}