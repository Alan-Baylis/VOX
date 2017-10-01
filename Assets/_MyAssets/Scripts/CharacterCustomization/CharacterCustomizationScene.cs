/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GameJolt.API;

namespace VOX{
	public class CharacterCustomizationScene : MonoBehaviour{
		
		public CharacterCustomization customization;

		[Header("Options Texts")]
		public TextMeshProUGUI skinText;
		public TextMeshProUGUI eyesText;
		public TextMeshProUGUI shirtText;
		public TextMeshProUGUI pantsText;
		public TextMeshProUGUI shoesText;

		CustomizationOptions options;

		void Start(){
			options = Resources.Load<CustomizationOptions>("CustomizationOptions");

			UpdateThings();
		}

		public void UpdateThings(){
			skinText.text = options.skinOptions[GameController.skin].name;
			eyesText.text = options.eyesOptions[GameController.eyes].name;
			shirtText.text = options.shirtOptions[GameController.shirt].name;
			pantsText.text = options.pantsOptions[GameController.pants].name;
			shoesText.text = options.shoesOptions[GameController.shoes].name;

			customization.SetColors();
			customization.UpdateTexture();

			GameController.characterTexture = customization.texture.EncodeToPNG();
		}

		public void UI_ChangeSkin(bool remove){
			GameController.skin = Mathf.Clamp(GameController.skin + (remove ? -1 : 1), 0, options.skinOptions.Length - 1);
			UpdateThings();
		}
		
		public void UI_ChangeEyes(bool remove){
			GameController.eyes = Mathf.Clamp(GameController.eyes + (remove ? -1 : 1), 0, options.eyesOptions.Length - 1);
			UpdateThings();
		}
		
		public void UI_ChangeShirt(bool remove){
			GameController.shirt = Mathf.Clamp(GameController.shirt + (remove ? -1 : 1), 0, options.shirtOptions.Length - 1);
			UpdateThings();
		}
		
		public void UI_ChangePants(bool remove){
			GameController.pants = Mathf.Clamp(GameController.pants + (remove ? -1 : 1), 0, options.pantsOptions.Length - 1);
			UpdateThings();
		}
		
		public void UI_ChangeShoes(bool remove){
			GameController.shoes = Mathf.Clamp(GameController.shoes + (remove ? -1 : 1), 0, options.shoesOptions.Length - 1);
			UpdateThings();
		}

		public void UI_Finish(){
			if(Manager.Instance != null && Manager.Instance.CurrentUser != null){
				DataStore.Set("character.skin", GameController.skin.ToString(), false);
				DataStore.Set("character.eyes", GameController.eyes.ToString(), false);
				DataStore.Set("character.shirt", GameController.shirt.ToString(), false);
				DataStore.Set("character.pants", GameController.pants.ToString(), false);
				DataStore.Set("character.shoes", GameController.shoes.ToString(), false);
				DataStore.Set("character.texture", System.Convert.ToBase64String(GameController.characterTexture), false);
			}

			SceneManager.LoadScene("MainMenu");
		}

		public void UI_Randomize(){
			GameController.skin = Random.Range(0, options.skinOptions.Length);
			GameController.eyes = Random.Range(0, options.eyesOptions.Length);
			GameController.shirt = Random.Range(0, options.shirtOptions.Length);
			GameController.pants = Random.Range(0, options.pantsOptions.Length);
			GameController.shoes = Random.Range(0, options.shoesOptions.Length);

			UpdateThings();
		}
	}
}