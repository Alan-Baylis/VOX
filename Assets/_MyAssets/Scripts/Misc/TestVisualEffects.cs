/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using Cirda.ImageEffects;
using UnityStandardAssets.ImageEffects;

namespace VOX{
	public class TestVisualEffects : MonoBehaviour{
		
		public float boxWidth = 128f;
		public float distanceFromSides = 16f;
		public float distanceFromBoxTop = 16f;
		public float distanceFromBoxSides = 4f;

		[Header("Visual Effects")]
		public Antialiasing antialiasing;
		public CameraMotionBlur cameraMotionBlur;
		public Tonemapping tonemapping;
		public SunShafts sunShafts;
		public GlitchEffect glitchEffect;
		public Pixelate pixelate;
		public BlackAndWhiteBlend blackAndWhiteBlend;

		void OnGUI(){
			if(MapController.hud.menu.activeInHierarchy){
				GUI.Box(new Rect(distanceFromSides, distanceFromSides, boxWidth, Screen.height - distanceFromSides * 2f), "Visual Effects");

				GUILayout.BeginArea(new Rect(
					distanceFromSides + distanceFromBoxSides,
					distanceFromSides + distanceFromBoxTop,
					boxWidth - distanceFromBoxSides * 2f,
					Screen.height - distanceFromSides * 2f - distanceFromBoxSides * 2f
				));
				
				antialiasing.enabled = GUILayout.Toggle(antialiasing.enabled, "Antialiasing");
				cameraMotionBlur.enabled = GUILayout.Toggle(cameraMotionBlur.enabled, "Motion Blur");
				tonemapping.enabled = GUILayout.Toggle(tonemapping.enabled, "Eye adaption");
				sunShafts.enabled = GUILayout.Toggle(sunShafts.enabled, "Sun Shafts");

				
				glitchEffect.enabled = GUILayout.Toggle(glitchEffect.enabled, "Glitch Effect");
				glitchEffect.displacementIntensity = HorizontalSlider(glitchEffect.displacementIntensity, 0f, 1f);
				glitchEffect.flipIntensity = HorizontalSlider(glitchEffect.flipIntensity, 0f, 1f);
				glitchEffect.colorIntensity = HorizontalSlider(glitchEffect.colorIntensity, 0f, 1f);

				pixelate.multiplier = HorizontalSlider(pixelate.multiplier, 1f, 0.005f, "Pixelate");
				pixelate.enabled = pixelate.multiplier < 1f;

				blackAndWhiteBlend.blend = HorizontalSlider(blackAndWhiteBlend.blend, 0f, 1f, "Black and White");
				blackAndWhiteBlend.enabled = blackAndWhiteBlend.blend > 0f;

				GUILayout.EndArea();
			}
		}

		float HorizontalSlider(float value, float leftValue, float rightValue, string label = ""){
			if(label != "")
				GUILayout.Label(label);

			return GUILayout.HorizontalSlider(value, leftValue, rightValue);
		}
	}
}