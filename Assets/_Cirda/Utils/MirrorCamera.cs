/*
* Copyright (c) Retardevs
* http://gamejolt.com/@retardevs
*/

using UnityEngine;

namespace Cirda.Utils{
     [RequireComponent(typeof(Camera))]
     [ExecuteInEditMode]
     public class MirrorCamera : MonoBehaviour{
		 
         public bool flipHorizontal;
         public bool flipVertical;

         Camera cam;

         void Awake(){
             cam = GetComponent<Camera>();
         }

         void OnPreCull(){
             cam.ResetWorldToCameraMatrix();
             cam.ResetProjectionMatrix();
             Vector3 scale = new Vector3(flipHorizontal ? -1 : 1, flipVertical ? -1 : 1, 1);
             cam.projectionMatrix = cam.projectionMatrix * Matrix4x4.Scale(scale);
         }

         void OnPreRender(){
             GL.invertCulling = (flipHorizontal && !flipVertical) || (!flipHorizontal && flipVertical);
         }
         
         void OnPostRender(){
             GL.invertCulling = false;
         }
     }
}