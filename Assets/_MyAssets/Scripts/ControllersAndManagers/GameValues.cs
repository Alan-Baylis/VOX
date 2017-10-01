/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	[CreateAssetMenu()]
	public class GameValues : ScriptableObject{

		public static GameValues Instance{
			get{ return Resources.Load<GameValues>("GameValues"); }
		}

		public string GameName;
		public string GameVersion;
		public bool AutoRun;
		public bool x64;

		public string PhotonGameVersion{
			get{ return GameName + "_" + GameVersion; }
		}

	}
}