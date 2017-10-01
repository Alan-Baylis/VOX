/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class DestroyOnStart : MonoBehaviour{
		
		public Object obj;
		public float delay;

		void Start(){
			if(obj != null){
				if(delay > 0f)
					Destroy(obj, delay);
				else
					Destroy(obj);
			}
		}
	}
}