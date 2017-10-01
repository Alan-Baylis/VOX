/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEditor;

namespace VOX{
	public class GameValuesEditor : EditorWindow{
		
		string gameName;
		string gameVersion;
		bool autoRun;
		bool x64;

		[MenuItem("VOX/Build &b")]
		static void Init(){
			GameValuesEditor window = GetWindow<GameValuesEditor>("Build");
			window.Show();
		}

		GameValues values;

		void Awake(){
			values = GameValues.Instance;
			
			gameName = values.GameName;
			gameVersion = values.GameVersion;
			autoRun = values.AutoRun;
			x64 = values.x64;
		}

		void OnGUI(){
			gameName = EditorGUILayout.TextField("Game Name", gameName);
			gameVersion = EditorGUILayout.TextField("Game Version", gameVersion);
			
			EditorGUILayout.Space();
			
			x64 = EditorGUILayout.Toggle("x64 Build", x64);
			
			if(GUILayout.Button("Build", GUILayout.Height(32f))){
				Save();
				BuildGame(x64 ? BuildTarget.StandaloneWindows64 : BuildTarget.StandaloneWindows, autoRun ? BuildOptions.AutoRunPlayer : BuildOptions.None);
			}

			autoRun = EditorGUILayout.Toggle("Run After Build", autoRun);
		}

		void Save(){
			values.GameName = gameName;
			values.GameVersion = gameVersion;
			values.AutoRun = autoRun;
			values.x64 = x64;

			EditorUtility.SetDirty(values);
			EditorApplication.SaveAssets();
			AssetDatabase.SaveAssets();
		}

		void BuildGame(BuildTarget target = BuildTarget.StandaloneWindows, BuildOptions options = BuildOptions.None){
			EditorBuildSettingsScene[] buildSettingsScenes = EditorBuildSettings.scenes;
			string[] scenes = new string[buildSettingsScenes.Length];
			for(int i = 0; i < scenes.Length; i++){
				scenes[i] = buildSettingsScenes[i].path;
			}

			string locationPathName = string.Format("Builds/{0}/{1}.exe", values.GameVersion, values.GameName);
			BuildPipeline.BuildPlayer(scenes, locationPathName, target, options);
		}
	}
}