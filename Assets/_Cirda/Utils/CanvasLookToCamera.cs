using UnityEngine;

[ExecuteInEditMode]
public class CanvasLookToCamera : MonoBehaviour{
	
	public Transform cam;
	public bool lockY;

	void LateUpdate(){
		if(cam == null)
			return;

		Vector3 rot = transform.position + cam.rotation * Vector3.forward;

		if(lockY)
			rot.y = transform.position.y;

		transform.LookAt(rot, cam.transform.rotation * Vector3.up);
	}
}