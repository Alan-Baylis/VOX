/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VOX{
	public class Nametag : MonoBehaviour{
		
		public CanvasLookToCamera canvasLookToCamera;
		public CameraRelativeScale cameraRelativeScale;

		[Header("UI")]
		public TextMeshProUGUI nametagText;
		
		public void UpdateNametag(string username){
			UpdateName(username);
			UpdateCamera();
		}
		public void UpdateName(string username){
			nametagText.text = username;
		}

		public void UpdateCamera(){
			Player cameraPlayer = ServerController.player;

			if(cameraPlayer != null){
				Camera cam = cameraPlayer.cam;
			
				canvasLookToCamera.cam = cam.transform;
				cameraRelativeScale.cam = cam.transform;
			}
		}
	}
}