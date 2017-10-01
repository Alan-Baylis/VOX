/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace VOX{
	public class HudExample : MonoBehaviour{
		
		public GameObject menu;
		public ClassCreation classCreation;
		public Scoreboard scoreboard;
		
		[Header("Crosshair")]
		public RectTransform crosshair;
		public Image crosshairImage;
		public CanvasGroup crosshairCP;
		public Animation crosshairAnim;
		public float crosshairSizeLerpSpeed = 10f;

		[Header("Hitmarker")]
		public AudioClip hitmarkerClip;
		public Color hitmarkerColor = Color.red;
		public float hitmarkerLerpSpeed = 5f;
		public float hitmarkerVolume = 0.1f;

		[Header("Ammo")]
		public TextMeshProUGUI ammoText;

		[Header("Health Bar")]
		public Transform healthBarMask;
		public Transform healthBar;
		
		[Header("Chat")]
		public Transform chat;
		public TMP_InputField chatField;
		public GameObject chatMessagePrefab;

		[Header("Killfeed")]
		public Transform killfeed;
		public GameObject killfeedItemPrefab;

		[Header("Flash")]
		public GameObject flash;
		public CanvasGroup flashCG;

		[Header("Hit")]
		public CanvasGroup hitCG;
		public float hitLerpSpeed = 5f;

		[Header("Timer")]
		public TextMeshProUGUI timerText;

		[Header("Audio")]
		public AudioMixer audioMixer;
		public AudioSource flashAudioSource;
		public AudioSource feedbackAudioSource;

		[Space]

		public AudioClip killSound;
		public AudioClip dieSound;

		public void UI_Resume(){
			menu.SetActive(false);
			MapController.optionsMenu.Close();
		}

		public void UI_ChangeClass(bool team = false){
			if(ServerController.HasRoundEnded)
				return;

			if(ServerController.gamemode != ServerController.Gamemode.TDM)
				team = false;

			Player player = ServerController.player;

			if(player != null){
				if(!player.isDead){
					player.DieCustomReason(string.Format("[CHANGING {0}]", team ? "TEAM" : "CLASS"), false);
				}else{
					player.CancelRespawn();
				}
			}

			classCreation.Open(team);
		}

		public void UI_Disconnect(){
			PhotonNetwork.LeaveRoom();
			SceneManager.LoadSceneAsync("MainMenu");
		}

		public void UI_Options(){
			MapController.optionsMenu.Open();
		}

		public void UI_Quit(){
			Application.Quit();
		}

		void Start(){
			ServerController.onKill += OnKill;
			ServerController.onDieCustomReason += OnDieCustomReason;
			ServerController.onChatMessage += OnChatMessage;

			menu.SetActive(false);
			chatField.gameObject.SetActive(false);
		}

		void OnDestroy(){
			if(ServerController.onKill != null)
				ServerController.onKill -= OnKill;

			if(ServerController.onDieCustomReason != null)
				ServerController.onDieCustomReason -= OnDieCustomReason;

			if(ServerController.onChatMessage != null)
				ServerController.onChatMessage -= OnChatMessage;
		}

		void Update(){
			if(!chatField.gameObject.activeInHierarchy){
				if(!classCreation.classes.activeInHierarchy){
					if(!menu.activeInHierarchy){
						if(Input.GetKeyDown(KeyCode.Escape)){
							menu.SetActive(true);
						}
						if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
							chatField.gameObject.SetActive(true);
						}
					}else{
						if(Input.GetKeyDown(KeyCode.Escape)){
							UI_Resume();
						}
					}
				}else{
					UI_Resume();
				}
			}else{
				if(!chatField.isFocused){
					chatField.Select();
					chatField.ActivateInputField();
				}

				if(Input.GetKeyDown(KeyCode.Escape)){
					chatField.text = "";
					chatField.gameObject.SetActive(false);
				}

				if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
					if(chatField.text != "")
						SendChatMessage(chatField.text);

					chatField.text = "";
					chatField.gameObject.SetActive(false);
				}
			}

			scoreboard.scoreboard.SetActive((Input.GetKey(KeyCode.Tab) && !GameController.IsPaused()) || (ServerController.HasRoundEnded && !menu.activeSelf));

			float time = (float) ServerController.RemainingTime;
			int minutes = Mathf.FloorToInt(time / 60);
			int seconds = Mathf.FloorToInt(time % 60);

			timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

			hitCG.alpha = Mathf.Lerp(hitCG.alpha, 0f, hitLerpSpeed * Time.deltaTime);
			crosshairImage.color = Color.Lerp(crosshairImage.color, Color.white, hitmarkerLerpSpeed * Time.deltaTime);

			Player player = ServerController.player;
			if(player != null){
				GunBase currentGunBase = player.items.currentGun;
				if(currentGunBase != null){
					GunInfo currentGunInfo = currentGunBase.info;
					
					bool isCrosshairHidden = player.isDead || (currentGunInfo.hideCrosshair && !player.reloading);
					bool hidingCrosshair = player.aiming;

					if(currentGunBase is Grenade){
						Grenade currentGrenade = (Grenade) currentGunBase;
						hidingCrosshair = currentGrenade.throwing;
					}

					float targetCrosshairAlpha = hidingCrosshair ? (currentGunInfo.showCrosshairWhileAiming ? 1f : 0f) : (isCrosshairHidden ? 0f : 1f);

					float multiplier = 1f;

					if(hidingCrosshair){
						multiplier = 0f;
					}else if(player.running){
						multiplier = currentGunInfo.runningMultiplier;
					}else if(player.crouching){
						multiplier = currentGunInfo.crouchingMultiplier;
					}else{
						multiplier = 1f;
					}

					Vector2 targetCrosshairSize = Vector2.one * (currentGunInfo.defaultCrosshairSize * multiplier);

					crosshairCP.alpha = Mathf.Lerp(crosshairCP.alpha, targetCrosshairAlpha, Gun.aimingLerpSpeed * Time.deltaTime);
					crosshair.sizeDelta = Vector2.Lerp(crosshair.sizeDelta, targetCrosshairSize, crosshairSizeLerpSpeed * Time.deltaTime);

					if(currentGunBase is Gun){
						Gun currentGun = currentGunBase as Gun;
					
						ammoText.gameObject.SetActive(true);

						if(currentGun.maxAmmo >= 0)
							ammoText.text = string.Format("{0}/{1}", currentGun.ammo, currentGun.maxAmmo);
						else
							ammoText.text = currentGun.ammo.ToString();

						if(player.reloading){
							float value = Mathf.InverseLerp(currentGun.reloadingT, currentGun.reloadingT + currentGunInfo.reloadTime, Time.time);
							crosshairCP.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.forward * 90f, value);
							crosshairCP.transform.localScale = Vector3.one;
						}
					}else if(currentGunBase is Grenade){
						Grenade currentGrenade = currentGunBase as Grenade;

						ammoText.gameObject.SetActive(true);
						ammoText.text = currentGrenade.grenades.ToString();
					}else if(currentGunBase is Knife){
						ammoText.gameObject.SetActive(false);
					}

					healthBarMask.localScale = new Vector3(Mathf.Clamp01((float) player.health / 100f), 1f, 1f);
				
					if(healthBarMask.localScale.x > 0f)
						healthBar.localScale = new Vector3(1f / healthBarMask.localScale.x, 1f, 1f);
				}
			}
		}

		public void ShowHitmarker(){
			feedbackAudioSource.PlayOneShot(hitmarkerClip, hitmarkerVolume);
			crosshairImage.color = hitmarkerColor;
		}

		public void SendChatMessage(string message){
			if(ServerController.player != null){
				ServerController.player.photonView.RPC("RPCSendChatMessage", PhotonTargets.All, message);
			}
		}

		public void StopCrosshairAnimation(){
			if(crosshairAnim == null)
				return;

			crosshairAnim.Stop();

			crosshairCP.transform.localEulerAngles = Vector3.zero;
			crosshairCP.transform.localScale = Vector3.one;
		}

		public void FinishCrosshairAnimation(){
			if(crosshairAnim == null)
				return;

			crosshairAnim.Play();
			crosshairCP.transform.localEulerAngles = Vector3.zero;
		}

		public void Flash(float flashDuration, float fadeOutDuration){
			StartCoroutine(IFlash(flashDuration, fadeOutDuration));
		}

		public void SmallFlash(){
			StartCoroutine(ISmallFlash());
		}

		public void Hit(int damage){
			hitCG.alpha = Mathf.Clamp(damage, 0f, 100f) / 100f;
		}

		public void CrosshairRecoill(int recoill){
			crosshair.sizeDelta = crosshair.sizeDelta + (Vector2.one * recoill);
		}


		
		KillfeedItem CreateKillfeedItem(){
			GameObject go = Instantiate(killfeedItemPrefab) as GameObject;
			go.transform.SetParent(killfeed);

			return go.GetComponent<KillfeedItem>();
		}

		ChatMessage CreateChatMessage(){
			GameObject go = Instantiate(chatMessagePrefab) as GameObject;
			go.transform.SetParent(chat);
			go.transform.SetAsLastSibling();

			return go.GetComponent<ChatMessage>();
		}



		public void OnKill(PhotonPlayer killer, GunInfo gun, PhotonPlayer dead, bool headshot){
			KillfeedItem item = CreateKillfeedItem();
			item.Set(killer, gun, dead, headshot);
			
			if(PhotonNetwork.player == killer){
				OnKill();
			}else if(PhotonNetwork.player == dead){
				OnDie();
			}
		}

		public void OnDieCustomReason(PhotonPlayer dead, string customReason){
			KillfeedItem item = CreateKillfeedItem();
			item.Set(dead, customReason);

			if(PhotonNetwork.player == dead){
				OnDie();
			}
		}

		public void OnChatMessage(PhotonPlayer player, string message){
			ChatMessage chatMessage = CreateChatMessage();
			chatMessage.Set(player, message);
		}


		
		void OnKill(){
			feedbackAudioSource.PlayOneShot(killSound);
		}

		void OnDie(){
			feedbackAudioSource.PlayOneShot(dieSound);
			StopFlash();
		}


		
		const float flashSoundVolume = 0.1f;
		const float smallFlashMultiplier = 0.25f;

		IEnumerator IFlash(float flashDuration, float fadeOutDuration){
			flash.SetActive(true);
			flashCG.alpha = 1f;
			
			audioMixer.SetFloat("FlashbangVolume", -80f);
			flashAudioSource.volume = flashSoundVolume;
			flashAudioSource.Play();
			
			float t = Time.time;
			while(Time.time < t + flashDuration){
				yield return 0;
			}

			float startTime = Time.time;
			while(Time.time < startTime + fadeOutDuration){
				float i = Mathf.InverseLerp(startTime + fadeOutDuration, startTime, Time.time);

				flashCG.alpha = i;
				audioMixer.SetFloat("FlashbangVolume", GameController.LinearToDecibel(1f - i));
				flashAudioSource.volume =  i * flashSoundVolume;
				yield return 0;
			}
			
			StopFlash();
		}

		IEnumerator ISmallFlash(){
			flash.SetActive(true);
			flashCG.alpha = 0.5f;
			
			audioMixer.SetFloat("FlashbangVolume", -80f * smallFlashMultiplier);
			flashAudioSource.volume = flashSoundVolume * smallFlashMultiplier;
			flashAudioSource.Play();

			float startTime = Time.time;
			while(Time.time < startTime + 0.5f){
				float i = Mathf.InverseLerp(startTime + 0.5f, startTime, Time.time);

				flashCG.alpha = i * 0.5f;
				audioMixer.SetFloat("FlashbangVolume", GameController.LinearToDecibel(1f - i) * smallFlashMultiplier);
				flashAudioSource.volume = i * (flashSoundVolume * smallFlashMultiplier);
				yield return 0;
			}
			
			StopFlash();
		}

		void StopFlash(){
			audioMixer.SetFloat("FlashbangVolume", 0f);
			flashAudioSource.Stop();

			flash.SetActive(false);
			flashCG.alpha = 0f;
		}
	}
}