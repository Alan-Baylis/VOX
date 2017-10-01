/*
* This script was made by Cirda Entertainment.
* Gamejolt: http://gamejolt.com/@cirdaent
*/

using UnityEngine;

namespace Cirda.Utils{
	public class EmitParticlesAndDestroy : MonoBehaviour{
		
		public ParticleSystem particleSystem;
		public int particlesToEmit = 5;
		public float lifeTime = 2f;

		void Start(){
			particleSystem.Emit(particlesToEmit);
			Destroy(gameObject, lifeTime);
		}
	}
}