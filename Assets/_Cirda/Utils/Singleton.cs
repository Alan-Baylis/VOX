/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

	protected static T _instance;

	public static T Instance{
		get{
			if(_instance == null){
				_instance = FindObjectOfType<T>();
					
				if(_instance == null){
					Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
				}
			}
				
			return _instance;
		}
	}

	void Awake(){
		if(_Persist()){
			_Init();
		}
	}

	bool _Persist(){
		if(_instance == null){
			_instance = this as T;
		}else if(_instance != this){
			Destroy(gameObject);
			return false;
		}

		if(gameObject.transform.parent == null){
			DontDestroyOnLoad(gameObject);
		}

		return true;
	}

	virtual protected void _Init(){}

}