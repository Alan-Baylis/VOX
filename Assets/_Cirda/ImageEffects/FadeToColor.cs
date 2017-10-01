/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Cirda.ImageEffects{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class FadeToColor : ImageEffectBase{
		
		public Color color = Color.white;
		[Range(0f, 1f)] public float blend = 0.75f;

		new void Start(){
			shader = Shader.Find("Hidden/FadeToColor");
		}
		
		public void OnRenderImage(RenderTexture source, RenderTexture destination){
			if(shader == null || material == null || blend == 0){
				if(shader == null){
					shader = Shader.Find("Hidden/FadeToColor");
				}

				Graphics.Blit(source, destination);
				return;
			}
 
			material.SetColor("_Color", color);
			material.SetFloat("_Blend", blend);
			Graphics.Blit(source, destination, material);
		}
	}
}