/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;

namespace VOX{
	[ExecuteInEditMode]
	public class CameraRectBorders : MonoBehaviour{
		
		public Camera cam;
		public Rect borders;

		void LateUpdate(){
			if(cam != null){
				//cam.rect = BordersRectToCameraRect(borders);
				
				float x = borders.x;
				float y = borders.height;
				float w = Screen.width - borders.width - x;
				float h = Screen.height - borders.y - y;

				cam.pixelRect = new Rect(x, y, w, h);
			}
		}
		
		public static Rect BordersRectToCameraRect(Rect rect){
			//float x = 1f - ((Screen.width - rect.x) / Screen.width);
			//float y = 1f - ((Screen.height - rect.height) / Screen.height) - h;

			//float w = ((Screen.width - rect.width) / Screen.width) - x;
			//float h = ((Screen.height - rect.y) / Screen.height);


			return new Rect();
		}

		public static Rect BordersRectToCameraRect(float x, float y, float w, float h){
			return BordersRectToCameraRect(new Rect(x, y, w, h));
		}
	}
}