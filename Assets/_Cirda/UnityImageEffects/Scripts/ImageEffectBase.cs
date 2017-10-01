using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects{
    [RequireComponent(typeof (Camera))]
    public class ImageEffectBase : MonoBehaviour{

        private Shader m_Shader;
        private Material m_Material;

        protected virtual void Start(){
            if(!SystemInfo.supportsImageEffects){
                enabled = false;
                return;
            }

            if(!m_Shader || !m_Shader.isSupported)
                enabled = false;
        }

        protected Material material{
            get{
                if(m_Material == null || (m_Material != null && m_Material.shader != m_Shader)){
                    m_Material = new Material(m_Shader);
                    m_Material.hideFlags = HideFlags.HideAndDontSave;
                }

                return m_Material;
            }
        }

        protected Shader shader{
            get{ return m_Shader; }
			set{ m_Shader = value; }
        }

        protected virtual void OnDisable(){
            if(m_Material){
                DestroyImmediate(m_Material);
            }
        }
    }
}
