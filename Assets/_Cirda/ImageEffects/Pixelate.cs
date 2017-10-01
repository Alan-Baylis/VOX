/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Cirda.ImageEffects{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class Pixelate : ImageEffectBase{

		[Range(0.005f, 1f)] public float multiplier = 0.5f;

		new void Start(){
			shader = Shader.Find("Hidden/DefaultEffectShader");
		}
		
		public void OnRenderImage(RenderTexture source, RenderTexture destination){
            if(shader == null || material == null || multiplier >= 1f){
				if(shader == null){
					shader = Shader.Find("Hidden/DefaultEffectShader");
				}

                Graphics.Blit(source, destination);
                return;
            }

			multiplier = Mathf.Clamp(multiplier, 0.005f, 1f);

			int width = Mathf.Clamp(Mathf.RoundToInt(Screen.width * multiplier), 1, Screen.width);
			int height = Mathf.Clamp(Mathf.RoundToInt(Screen.height * multiplier), 1, Screen.height);

			if(material){
				RenderTexture scaled = RenderTexture.GetTemporary(width, height);
				scaled.filterMode = FilterMode.Point;

				Graphics.Blit(source, scaled, material);
				Graphics.Blit(scaled, destination);

				RenderTexture.ReleaseTemporary(scaled);
			}else{
				Graphics.Blit(source, destination);
			}
		}
	}
}