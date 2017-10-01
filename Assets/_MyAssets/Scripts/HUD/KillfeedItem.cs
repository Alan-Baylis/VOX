/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace VOX{
	public class KillfeedItem : MonoBehaviour{
		
		public float lifeTime = 5f;
		public AnimationClip disappearAnimationClip;

		[Header("UI")]
		public Animator anim;
		public TextMeshProUGUI killerText;
		public TextMeshProUGUI customReasonText;
		public TextMeshProUGUI deadText;
		public GameObject headshotIcon;

		[Header("Weapon Icon")]
		public LayoutElement weaponLayoutElement;
		public Image weaponIcon;

		[Header("Team Colors")]
		public Color noneTeamColor = Color.white;
		public Color redTeamColor = Color.red;
		public Color blueTeamColor = Color.cyan;

		void Start(){
			StartCoroutine(IAnimate());
		}

		public void Set(PhotonPlayer killer, GunInfo gun, PhotonPlayer dead, bool headshot){
			killerText.gameObject.SetActive(true);
			weaponIcon.gameObject.SetActive(true);
			customReasonText.gameObject.SetActive(false);
			headshotIcon.SetActive(headshot);

			killerText.text = killer.NickName;
			killerText.color = ServerController.GetObjectFromTeam<Color>(killer.GetTeam(), redTeamColor, blueTeamColor, noneTeamColor);

			float ratio = gun.sprite.rect.width / gun.sprite.rect.height;
			weaponIcon.sprite = gun.sprite;
			weaponLayoutElement.preferredWidth = weaponLayoutElement.preferredHeight * ratio;

			deadText.text = dead.NickName;
			deadText.color = ServerController.GetObjectFromTeam<Color>(dead.GetTeam(), redTeamColor, blueTeamColor, noneTeamColor);
		}

		public void Set(PhotonPlayer dead, string customReason){
			killerText.gameObject.SetActive(false);
			weaponIcon.gameObject.SetActive(false);
			customReasonText.gameObject.SetActive(true);
			headshotIcon.SetActive(false);

			customReasonText.text = customReason;

			deadText.text = dead.NickName;
			deadText.color = ServerController.GetObjectFromTeam<Color>(dead.GetTeam(), redTeamColor, blueTeamColor, noneTeamColor);
		}

		IEnumerator IAnimate(){
			yield return new WaitForSeconds(lifeTime);

			anim.SetTrigger("Disappear");
			Destroy(gameObject, disappearAnimationClip.length);
		}
	}
}