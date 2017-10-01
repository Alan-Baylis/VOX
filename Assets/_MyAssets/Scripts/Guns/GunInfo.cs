/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace VOX{
	[CreateAssetMenu()]
	public class GunInfo : ScriptableObject{
		
		public enum GunType{ Primary = 0, Secondary = 1, Melee = 2, Item = 3 }

		public int ID;
		public string Name;
		public string KillfeedName;
		public bool Custom;

		[Header("Item")]
		public GunType type;
		public Sprite sprite;

		[Header("Models")]
		public GameObject viewmodel;
		public GameObject worldmodel;

		[Header("Weapon Stats")]
		public bool auto = true;
		public int shots = 1;
		public int ammo = 30;
		public int startMags = 4;
		public float reloadTime = 1f;
		public float physicsForce = 5f;

		[Space]

		public float RPM = 600f;
		public float aimedRPM = 600f;

		[Space]

		public float coneAngle = 5f;
		public float aimedConeAngle = 3f;

		[Space]

		public Damage damage;

		[Space]

		public Recoill recoill;
		public Recoill aimedRecoill;

		[Header("Crosshair")]
		public int defaultCrosshairSize = 32;
		public int crosshairRecoil = 8;

		[Header("Multipliers")]
		public float runningMultiplier = 1.5f;
		public float crouchingMultiplier = 0.75f;

		[Header("Aiming")]
		public float aimedFov = 10f;

		[Header("Animations")]
		public AnimationClip drawClip;
		public AnimationClip idleClip;
		public AnimationClip fireClip;
		public AnimationClip reloadClip;

		[Space]

		public Location normalLoc = new Location(1f);
		public Location runningLoc = new Location(1f);
		public Location aimingLoc = new Location(1f);

		[Header("Sounds")]
		public AudioClip fireSound;

		[Header("Misc")]
		public bool disableHeadshots;
		public bool hideCrosshair;
		public bool showCrosshairWhileAiming;



		public float RPMToFireRate(float rpm){
			return 1f / (rpm / 60f);
		}

		public int startMaxAmmo{
			get{ return ammo * startMags; }
		}
	}

	[System.Serializable]
	public struct Damage{
		public int head;
		public int torso;
		public int arms;
		public int legs;
	}

	[System.Serializable]
	public struct Recoill{
		public float recoill;
		[Range(0f, 1f)] public float divider;
		public float lerpSpeed;
	}

	[System.Serializable]
	public struct Location{
		public Vector3 position;
		public Vector3 eulerAngles;
		public Vector3 scale;
		
		public Location(Vector3 pos, Vector3 rot, Vector3 scl){
			position = pos;
			eulerAngles = rot;
			scale = scl;
		}

		public Location(float scl){
			position = Vector3.zero;
			eulerAngles = Vector3.zero;
			scale = Vector3.one * scl;
		}

		public Quaternion rotation{
			get{ return Quaternion.Euler(eulerAngles); }
			set{ eulerAngles = value.eulerAngles; }
		}
	}
}