/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Cirda.ImageEffects{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class GlitchEffect : ImageEffectBase{

		public Texture2D displacementMap;
		float glitchup, glitchdown, flicker, glitchupTime = 0.05f, glitchdownTime = 0.05f, flickerTime = 0.5f;

		[Header("Glitch Intensity")]
		[Range(0f, 1f)] public float displacementIntensity;
		[Range(0f, 1f)] public float flipIntensity;
		[Range(0f, 1f)] public float colorIntensity;

		new void Start(){
			shader = Shader.Find("Hidden/GlitchEffectShader");
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination){
		#if UNITY_EDITOR
			if(!UnityEditor.EditorApplication.isPlaying){
                Graphics.Blit(source, destination);
                return;
			}
		#endif

            if(shader == null || material == null || (displacementIntensity == 0f && flipIntensity == 0f && colorIntensity == 0f)){
				if(shader == null){
					shader = Shader.Find("Hidden/GlitchEffectShader");
				}

                Graphics.Blit(source, destination);
                return;
            }
			
			displacementIntensity = Mathf.Clamp01(displacementIntensity);
			flipIntensity = Mathf.Clamp01(flipIntensity);
			colorIntensity = Mathf.Clamp01(colorIntensity);

			material.SetFloat("_Intensity", displacementIntensity);
			material.SetFloat("_ColorIntensity", colorIntensity);
			material.SetTexture("_DispTex", displacementMap);
        
			flicker += Time.deltaTime * colorIntensity;
			if(flicker > flickerTime){
				material.SetFloat("filterRadius", Random.Range(-3f, 3f) * colorIntensity);
				material.SetVector("direction", Quaternion.AngleAxis(Random.Range(0, 360) * colorIntensity, Vector3.forward) * Vector4.one);
				flicker = 0;
				flickerTime = Random.value;
			}

			if(colorIntensity == 0)
				material.SetFloat("filterRadius", 0);
        
			glitchup += Time.deltaTime * flipIntensity;
			if(glitchup > glitchupTime){
				if(Random.value < 0.1f * flipIntensity)
					material.SetFloat("flip_up", Random.Range(0, 1f) * flipIntensity);
				else
					material.SetFloat("flip_up", 0);
			
				glitchup = 0;
				glitchupTime = Random.value/10f;
			}

			if(flipIntensity == 0)
				material.SetFloat("flip_up", 0);

			glitchdown += Time.deltaTime * flipIntensity;
			if(glitchdown > glitchdownTime){
				if(Random.value < 0.1f * flipIntensity)
					material.SetFloat("flip_down", 1 - Random.Range(0, 1f) * flipIntensity);
				else
					material.SetFloat("flip_down", 1);
			
				glitchdown = 0;
				glitchdownTime = Random.value/10f;
			}

			if(flipIntensity == 0)
				material.SetFloat("flip_down", 1);

			if(Random.value < 0.05 * displacementIntensity){
				material.SetFloat("displace", Random.value * displacementIntensity);
				material.SetFloat("scale", 1 - Random.value * displacementIntensity);
			}else{
				material.SetFloat("displace", 0);
			}
		
			Graphics.Blit(source, destination, material);
		}
	}
}