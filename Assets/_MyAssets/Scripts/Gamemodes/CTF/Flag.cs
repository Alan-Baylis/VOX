/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class Flag : MonoBehaviour{
		
		/*
		public ServerController.CTFFlag team;
		public FlagVisual visual;

		void Start(){
			if(ServerController.gamemode != ServerController.Gamemode.CTF){
				Destroy(gameObject);
				return;
			}

			visual.RefreshModel(team);

			ServerController.onPickupFlag += OnPickupFlag;
			ServerController.onReturnFlag += OnReturnFlag;
		}

		void OnValidate(){
			visual.RefreshModel(team);
		}

		public void OnTouch(PhotonView view){
			PhotonPlayer player = view.owner;
			PunTeams.Team playerTeam = player.GetTeam();

			bool isSameTeam = (
				(playerTeam == PunTeams.Team.red && team == ServerController.CTFFlag.Red) ||
				(playerTeam == PunTeams.Team.blue && team == ServerController.CTFFlag.Blue)
			);

			bool isTaken = (
				(team == ServerController.CTFFlag.Red && ServerController.redFlagOwner != null) ||
				(team == ServerController.CTFFlag.Blue && ServerController.blueFlagOwner != null)
			);

			bool isHoldingAnyFlag = ServerController.IsHoldingAnyFlag(view);

			if(!isSameTeam){
				if(!isTaken && !isHoldingAnyFlag){
					ServerController.PickupFlag(view, team);
					return;
				}
			}else{
				if(isHoldingAnyFlag){
					ServerController.ReturnFlag(team);
					ServerController.AddPoint(player);
					return;
				}
			}
		}

		public void OnPickupFlag(PhotonView view, ServerController.CTFFlag flag){
			if(flag == team){
				visual.gameObject.SetActive(false);
			}
		}

		public void OnReturnFlag(ServerController.CTFFlag flag){
			if(flag == team){
				visual.gameObject.SetActive(true);
			}
		}
		*/
	}
}