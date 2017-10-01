/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Cirda.ImageEffects{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class MultiplyToColor : ImageEffectBase{
		
		public Color color = Color.red;
		[Range(0f, 1f)] public float blend = 0.5f;

		new void Start(){
			shader = Shader.Find("Hidden/MultiplyToColor");
		}
		
		public void OnRenderImage(RenderTexture source, RenderTexture destination){
			if(shader == null || material == null){
				if(shader == null){
					shader = Shader.Find("Hidden/MultiplyToColor");
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