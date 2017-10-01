/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace VOX{
	public class AlwaysOnTopHUD : MonoBehaviour{
		
		public TextMeshProUGUI versionText;

		[Header("FPS Counter")]
		public TextMeshProUGUI fpsCounterText;
		public float fpsCounterUpdateRate = 0.5f;

		void Start(){
			versionText.text = GameValues.Instance.GameVersion;
			StartCoroutine(IFPSCounter());
		}

		IEnumerator IFPSCounter(){
			while(true){
				int lastFrameCount = Time.frameCount;
				float lastTime = Time.realtimeSinceStartup;

				while(Time.realtimeSinceStartup < lastTime + fpsCounterUpdateRate){
					yield return 0;
				}

				float timeSpan = Time.realtimeSinceStartup - lastTime;
				int frameCount = Time.frameCount - lastFrameCount;
 
				int fps = Mathf.RoundToInt(frameCount / timeSpan);

				if(PhotonNetwork.inRoom){
					int ping = PhotonNetwork.GetPing();
					fpsCounterText.text = string.Format("{0} FPS\n{1} PING", fps, ping);
				}else{
					fpsCounterText.text = string.Format("{0} FPS", fps);
				}
			}
		}
	}
}