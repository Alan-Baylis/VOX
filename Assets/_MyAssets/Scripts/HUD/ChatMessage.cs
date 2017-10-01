/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using TMPro;
using System.Collections;

namespace VOX{
	public class ChatMessage : MonoBehaviour{
		
		public string chatFormat = "<color={2}><b>{0}</b></color>: {1}";
		
		[Header("Animation")]
		public float lifeTime = 10f;
		public Animator anim;
		public AnimationClip disappearAnimationClip;

		[Header("Colors")]
		public string noneTeamColor = "white";
		public string redTeamColor = "red";
		public string blueTeamColor = "blue";
		
		[Header("ChatMessage")]
		public TextMeshProUGUI text;

		void Start(){
			StartCoroutine(IAnimate());
		}

		public void Set(PhotonPlayer player, string message){
			string color = ServerController.GetObjectFromTeam<string>(player.GetTeam(), redTeamColor, blueTeamColor, noneTeamColor);

			text.text = string.Format(chatFormat, player.NickName, message, color);
		}

		IEnumerator IAnimate(){
			yield return new WaitForSeconds(lifeTime);

			anim.SetTrigger("Disappear");
			Destroy(gameObject, disappearAnimationClip.length);
		}

	}
}