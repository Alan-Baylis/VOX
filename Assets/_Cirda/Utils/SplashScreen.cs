/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Cirda.Utils{
	public class SplashScreen : MonoBehaviour{
		
		public MovieTexture video;
		public RawImage rawImage;
		public AudioSource source;

		[Header("Values")]
		public float timeToChangeScene = 5f;
		public string sceneToLoad = "MainMenu";

		void Start(){
			StartCoroutine("IPlay");
		}

		void Update(){
			if(Input.anyKeyDown){
				StopCoroutine("IPlay");
				SceneManager.LoadScene(sceneToLoad);
			}
		}

		IEnumerator IPlay(){
			rawImage.texture = video;
			source.clip = video.audioClip;
			
			rawImage.enabled = true;
			source.Play();
			video.Play();

			yield return new WaitForSeconds(timeToChangeScene);

			SceneManager.LoadScene(sceneToLoad);
		}
	}
}