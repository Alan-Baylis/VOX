/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	public class PlayerMouseLook : Photon.MonoBehaviour{
		
		public Transform mouseLookRoot;

		public float xRot{
			get{ return _xRot; }
			set{ _xRot = value; }
		}

		float _xRot;
		float recoill;
		float recoillLerpSpeed = 10f;

		void Update(){
			if(photonView.isMine){
				recoill = Mathf.Lerp(recoill, 0f, recoillLerpSpeed * Time.deltaTime);

				if(!GameController.IsPaused()){
					float yRot = Input.GetAxis("Mouse X") * Settings.Sensitivity;
					_xRot -= Input.GetAxis("Mouse Y") * Settings.Sensitivity;
					_xRot = Mathf.Clamp(_xRot, -90f, 90f);

					transform.Rotate(0f, yRot, 0f);
				}

				mouseLookRoot.localEulerAngles = new Vector3(Mathf.Clamp(_xRot - recoill, -999f, 999f), 0f, 0f);
			}
		}

		public void Recoill(Recoill recoill){
			if(photonView.isMine){
				this.recoill += recoill.recoill * recoill.divider;
				recoillLerpSpeed = recoill.lerpSpeed;

				_xRot = Mathf.Clamp(_xRot - recoill.recoill * (1f - recoill.divider), -999f, 999f);
			}
		}
	}
}