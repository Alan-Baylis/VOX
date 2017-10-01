using UnityEngine;

public class ChangeScaleOnStart : MonoBehaviour{

	public Vector3 scaleToSet = Vector3.one;

	void Start(){
		transform.localScale = scaleToSet;
	}

}