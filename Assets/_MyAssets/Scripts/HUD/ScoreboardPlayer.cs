/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon;
using ExitGames.Client.Photon;
using System.Collections;

namespace VOX{
	public class ScoreboardPlayer : PunBehaviour{
		
		public Scoreboard scoreboard;
		public PhotonPlayer player;

		[Header("UI")]
		public Image back;
		public RawImage avatar;
		public TextMeshProUGUI usernameText;
		public TextMeshProUGUI scoreText;

		[Header("Colors")]
		public Color defaultColor = Color.white;
		public Color ownerColor = Color.white;

		bool needsToDownloadImage = true;
		bool needsToRefresh = false;

		void Awake(){
			ServerController.onRefreshScoreboard += Refresh;
		}

		void OnDestroy(){
#if UNITY_EDITOR
			if(!UnityEditor.EditorApplication.isPlaying)
				return;
#endif

			if(ServerController.onRefreshScoreboard != null)
				ServerController.onRefreshScoreboard -= Refresh;
		}

		void OnEnable(){
			if(player != null){
				if(needsToDownloadImage){
					StartCoroutine(IDownloadAvatar());
				}

				if(needsToRefresh){
					Refresh();
				}
			}
		}

		public void Refresh(){
			if(!gameObject.activeInHierarchy){
				needsToRefresh = true;
				return;
			}

			needsToRefresh = false;

			transform.SetParent(
				ServerController.GetObjectFromTeam<GameObject>(
					player.GetTeam(), scoreboard.redTeamGroup, scoreboard.blueTeamGroup, scoreboard.noneTeamGroup
				).transform
			);

			transform.localScale = Vector3.one;

			back.color = PhotonNetwork.player == player ? ownerColor : defaultColor;

			usernameText.text = player.NickName;
			
			int wins = ServerController.GetPoints(player);
			int kills = ServerController.GetKills(player);
			int deaths = ServerController.GetDeaths(player);

			scoreText.text = string.Format("{0}/{1}/{2}", kills, deaths, wins);
		}

		IEnumerator IDownloadAvatar(){
			ExitGames.Client.Photon.Hashtable props = player.CustomProperties;
			WWW www = new WWW((string) props["avatarURL"]);

			yield return www;

			Texture2D tex = new Texture2D(60, 60, TextureFormat.RGB24, false);
			www.LoadImageIntoTexture(tex);

			avatar.texture = tex;

			needsToDownloadImage = false;
		}
	}
}