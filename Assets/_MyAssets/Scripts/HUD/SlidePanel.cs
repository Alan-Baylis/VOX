/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;

namespace VOX{
	[ExecuteInEditMode]
	public class SlidePanel : MonoBehaviour{

		public enum Side{
			Left,
			Right,
			Top,
			Down
		}

		[Range(0f, 1f)] public float t = 1f;
		public Side side;

		[Header("UI")]
		public RectTransform rectTransform;


		void Update(){
			if(rectTransform != null){
				rectTransform.anchoredPosition = Vector2.Lerp(GetDifferenceToOutside(), Vector2.zero, Mathf.Clamp01(t));
			}
		}

		Vector2 GetDifferenceToOutside(){
			Rect rect = rectTransform.rect;

			if(side == Side.Left)
				return new Vector2(-rect.width, 0f);
			else if(side == Side.Right)
				return new Vector2(rect.width, 0f);
			else if(side == Side.Top)
				return new Vector2(0f, rect.height);
			else
				return new Vector2(0f, -rect.height);
		}
	}
}