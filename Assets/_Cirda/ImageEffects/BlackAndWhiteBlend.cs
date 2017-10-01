/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Cirda.ImageEffects{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class BlackAndWhiteBlend : ImageEffectBase{

		[Range(0f, 1f)] public float blend = 1f;

		new void Start(){
			shader = Shader.Find("Hidden/BlackAndWhiteBlend");
		}
		
		public void OnRenderImage(RenderTexture source, RenderTexture destination){
			if(shader == null || material == null || blend == 0){
				if(shader == null){
					shader = Shader.Find("Hidden/BlackAndWhiteBlend");
				}

				Graphics.Blit(source, destination);
				return;
			}
 
			material.SetFloat("_Blend", blend);
			Graphics.Blit(source, destination, material);
		}
	}
}