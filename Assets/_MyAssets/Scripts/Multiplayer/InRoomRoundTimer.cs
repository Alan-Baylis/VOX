using UnityEngine;
using Photon;
using ExitGames.Client.Photon;
using System;

public class InRoomRoundTimer : PunBehaviour{

    double startTime;
	double timePerRound;

	[HideInInspector] public double elapsedTime = 1d;
	[HideInInspector] public double remainingTime = 1d;

	public bool hasRoundEnded{
		get{ return remainingTime < 1d; }
	}
	
	static bool hasCalledOnRoundEndAction;
	public Action onRoundEnd;

	void Awake(){
        if(PhotonNetwork.isMasterClient){
            StartRoundTime();
        }
	}

    void Update(){
		if(timePerRound == 0d){
			timePerRound = (double) PhotonNetwork.room.CustomProperties["timePerRound"];

			if(timePerRound < 0d){
				enabled = false;
			}
		}

		if(Input.GetKeyDown(KeyCode.P) && PhotonNetwork.isMasterClient){
			SetTime(5d);
		}

		if(!hasRoundEnded){
			elapsedTime = (PhotonNetwork.time - startTime);
			remainingTime = timePerRound - elapsedTime;
		}else{
			if(!hasCalledOnRoundEndAction)
				EndRound();

			elapsedTime = 0f;
			remainingTime = 0f;
		}
    }

    public void StartRoundTime(){
		hasCalledOnRoundEndAction = false;

		if(PhotonNetwork.isMasterClient){
			SetTime(timePerRound);
		}
    }

	public void EndRound(){
		if(onRoundEnd != null && !hasCalledOnRoundEndAction){
			onRoundEnd();
		}

		hasCalledOnRoundEndAction = true; 
	}

	public void SetTime(double time){
		if(PhotonNetwork.isMasterClient){
			Hashtable props = new Hashtable(){{"startTime", PhotonNetwork.time - (timePerRound - (time + 1d))}};
			PhotonNetwork.room.SetCustomProperties(props);
		}
	}

    public override void OnPhotonCustomRoomPropertiesChanged(Hashtable props){
        if(props.ContainsKey("startTime")){
            startTime = (double) props["startTime"];
        }
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient){
        if(!PhotonNetwork.room.CustomProperties.ContainsKey("startTime")){
            StartRoundTime();
        }
    }

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		if(PhotonNetwork.isMasterClient){
			SetTime(remainingTime);
		}
	}
}
