/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

namespace VOX{
	public static class Loading{

		const string loadingSceneName = "LoadingScene";
		
		public static string SceneName;
		public static string MapName;
		public static string MapSubtitle;
		public static string MapDescription;

		public static Texture2D MapIcon;
		public static Texture2D MapBackground;

		public static AsyncOperation SceneAsyncOperation;

		public static void LoadMap(string sceneName, string mapName, string mapSubtitle = "", string mapDescription = ""){
			SceneName = sceneName;
			MapName = mapName;
			MapSubtitle = mapSubtitle;
			MapDescription = mapDescription;

			SceneManager.LoadScene(loadingSceneName);
		}

		public static void StartLoadingScene(){
			SceneAsyncOperation = SceneManager.LoadSceneAsync(SceneName);
			SceneAsyncOperation.allowSceneActivation = false;
		}

		public static void FinishLoading(){
			SceneAsyncOperation.allowSceneActivation = true;

			SceneName = string.Empty;
			MapName = string.Empty;
			MapSubtitle = string.Empty;
			MapDescription = string.Empty;

			MapIcon = null;
			MapBackground = null;
		}
	}
}