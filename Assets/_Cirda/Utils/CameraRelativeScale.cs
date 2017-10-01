using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraRelativeScale : MonoBehaviour{

	public Transform cam;
	public Vector3 initialScale = Vector3.one;
	public float multiplier = 0.5f;

	void LateUpdate(){
		if(cam == null)
			return;

		Plane plane = new Plane(cam.forward, cam.position);
		float dist = plane.GetDistanceToPoint(transform.position);

		transform.localScale = initialScale * dist * multiplier;
	}
}