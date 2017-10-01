/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace Cirda.Utils{
	public class TestNavMeshAgent : MonoBehaviour{

		public NavMeshAgent agent;
		public Transform destination;

		void Update(){
			if(destination != null)
				agent.SetDestination(destination.position);
		}

	}
}