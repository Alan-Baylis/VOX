/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using GameJolt.API;
using Photon;
using System.Collections;

namespace VOX{
	public class CharacterCustomization : PunBehaviour{
		
		public bool startUpOnAwake = true;
		public bool loadTextureFromProperties = true;
		public Renderer[] renderers;

		public Texture2D texture;

		Material mat;
		CustomizationOptions options;
		
		int skin;
		int eyes;
		int shirt;
		int pants;
		int shoes;

		void Awake(){
			options = Resources.Load<CustomizationOptions>("CustomizationOptions");

			if(startUpOnAwake){
				StartUp();
			}
		}

		public void StartUp(){
			if(!loadTextureFromProperties){
				LoadColors();
			}

			UpdateMaterials();
			UpdateTexture();
		}

		public void LoadColors(){
			PhotonView view = PhotonView.Get(transform.root);

			if(view != null){
				ExitGames.Client.Photon.Hashtable hash = view.owner.CustomProperties;

				if(hash != null && hash.Count > 0){
					skin = (int) hash["character.skin"];
					eyes = (int) hash["character.eyes"];
					shirt = (int) hash["character.shirt"];
					pants = (int) hash["character.pants"];
					shoes = (int) hash["character.shoes"];
				}
			}else{
				skin = GameController.skin;
				eyes = GameController.eyes;
				shirt = GameController.shirt;
				pants = GameController.pants;
				shoes = GameController.shoes;
			}
		}

		public void UpdateMaterials(){
			mat = renderers[0].material;

			for(int i = 1; i < renderers.Length; i++){
				renderers[i].material = mat;
			}
		}

		public void SetColors(int skin, int eyes, int shirt, int pants, int shoes){
			this.skin = skin;
			this.eyes = eyes;
			this.shirt = shirt;
			this.pants = pants;
			this.shoes = shoes;
		}

		public void SetColors(){
			SetColors(GameController.skin, GameController.eyes, GameController.shirt, GameController.pants, GameController.shoes);
		}

		public void UpdateTexture(){
			texture = CreateTexture();
			mat.mainTexture = texture;
		}

		public Texture2D CreateTexture(){
			Texture2D newTex = new Texture2D(options.defaultTexture.width, options.defaultTexture.height, options.defaultTexture.format, false);
			newTex.filterMode = FilterMode.Point;

			if(loadTextureFromProperties){
				PhotonView view = PhotonView.Get(transform.root);
				ExitGames.Client.Photon.Hashtable hash = view.owner.CustomProperties;
				newTex.LoadImage((byte[]) hash["character.texture"]);
			}else{
				for(int x = 0; x < options.defaultTexture.width; x++){
					for(int y = 0; y < options.defaultTexture.height; y++){
						Color pixel = options.defaultTexture.GetPixel(x, y);
					
						if(pixel == options.skinColor){
							newTex.SetPixel(x, y, options.skinOptions[skin].color);
						}else if(pixel == options.eyesColor){
							newTex.SetPixel(x, y, options.eyesOptions[eyes].color);
						}else if(pixel == options.shirtColor){
							newTex.SetPixel(x, y, options.shirtOptions[shirt].color);
						}else if(pixel == options.pantsColor){
							newTex.SetPixel(x, y, options.pantsOptions[pants].color);
						}else if(pixel == options.shoesColor){
							newTex.SetPixel(x, y, options.shoesOptions[shoes].color);
						}else{
							newTex.SetPixel(x, y, Color.white);
						}
					}
				}

				newTex.Apply(false, false);
			}
			return newTex;
		}
	}
}