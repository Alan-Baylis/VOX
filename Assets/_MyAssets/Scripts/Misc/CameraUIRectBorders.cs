/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;

namespace VOX{
	[ExecuteInEditMode]
	public class CameraUIRectBorders : MonoBehaviour{
		
		public Camera cam;
		public RectTransform rectTransform;

		void LateUpdate(){
			if(cam != null && rectTransform != null){
				float w = rectTransform.rect.width;
				float h = rectTransform.rect.height;
				float x = rectTransform.transform.position.x - w;
				float y = rectTransform.transform.position.y - h;

				cam.pixelRect = new Rect(x, y, w, h);
			}
		}
	}
}