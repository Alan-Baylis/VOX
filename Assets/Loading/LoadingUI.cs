/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Photon;

namespace VOX{
	public class LoadingUI : PunBehaviour{

		public float startDelay = 1f;

		[Header("UI")]
		public Transform loadingBar;
		public TextMeshProUGUI infoText;
		public RawImage mapIcon;
		public RawImage mapBackground;

		void Start(){
			StartCoroutine(ILoading());
		}

		void Update(){
			if(Loading.SceneAsyncOperation != null && Loading.SceneAsyncOperation.isDone){
				loadingBar.localScale = new Vector3(Mathf.Clamp01(Loading.SceneAsyncOperation.progress), 1f, 1f);
			}
		}

		IEnumerator ILoading(){
			string infoFormat = infoText.text;

			infoText.text = string.Format(infoFormat, Loading.MapName, Loading.MapSubtitle, Loading.MapDescription);
			mapIcon.texture = Loading.MapIcon;
			mapBackground.texture = Loading.MapBackground;

			yield return new WaitForSeconds(startDelay);
			
			Loading.StartLoadingScene();
			
			yield return Loading.SceneAsyncOperation.isDone;

			Loading.FinishLoading();
		}
	}
}