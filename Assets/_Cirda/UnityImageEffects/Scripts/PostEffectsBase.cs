using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    public class PostEffectsBase : MonoBehaviour
	{
        protected bool supportHDRTextures = true;
        protected bool supportDX11 = false;
        protected bool isSupported = true;

        private List<Material> createdMaterials = new List<Material> ();

		void OnEnable(){
			supportHDRTextures = true;
			supportDX11 = true;
			isSupported = true;
			CheckSupport();
		}
			
		protected void Start(){
			CheckResources();
		}

        protected Material CheckShaderAndCreateMaterial ( Shader s, Material m2Create)
		{
            if (!s)
			{
				//#if !UNITY_EDITOR
				//enabled = false;
				//#endif

                return null;
            }

            if (s.isSupported && m2Create && m2Create.shader == s)
                return m2Create;

            if (!s.isSupported)
			{
                NotSupported ();
                return null;
            }

            m2Create = new Material (s);
            createdMaterials.Add (m2Create);
            m2Create.hideFlags = HideFlags.DontSave;

            return m2Create;
		}


        protected Material CreateMaterial (Shader s, Material m2Create)
		{
            if (!s)
			{
                return null;
            }

            if (m2Create && (m2Create.shader == s) && (s.isSupported))
                return m2Create;

            if (!s.isSupported)
			{
                return null;
            }

            m2Create = new Material (s);
            createdMaterials.Add (m2Create);
            m2Create.hideFlags = HideFlags.DontSave;
                
            return m2Create;
		}

        void OnDestroy ()
        {
            RemoveCreatedMaterials ();    
        }

        private void RemoveCreatedMaterials ()
        {
            while (createdMaterials.Count > 0)
            {
                Material mat = createdMaterials[0];
                createdMaterials.RemoveAt (0);
#if UNITY_EDITOR
                DestroyImmediate (mat);
#else
                Destroy(mat);
#endif
            }
        }


        protected bool CheckSupport ()
		{
            return CheckSupport(false);
        }


        public virtual bool CheckResources ()
		{
            return isSupported;
        }

        protected bool CheckSupport (bool needDepth)
		{
            isSupported = true;
            supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;

            if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
			{
                NotSupported ();
                return false;
            }

            if (needDepth && !SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth))
			{
                NotSupported ();
                return false;
            }

            if (needDepth)
                GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;

            return true;
        }

        protected bool CheckSupport (bool needDepth,  bool needHdr)
		{
            if (!CheckSupport(needDepth))
                return false;

            if (needHdr && !supportHDRTextures)
			{
                NotSupported ();
                return false;
            }

            return true;
        }


        public bool Dx11Support ()
		{
            return supportDX11;
        }


        protected void ReportAutoDisable ()
		{
        }

        // deprecated but needed for old effects to survive upgrading
        bool CheckShader (Shader s)
		{
            if (!s.isSupported)
			{
                NotSupported ();
                return false;
            }
            else
			{
                return false;
            }
        }


        protected void NotSupported ()
		{
			//#if !UNITY_EDITOR
			//enabled = false;
			//#endif

            isSupported = false;
            return;
        }


        protected void DrawBorder (RenderTexture dest, Material material)
		{
            float x1;
            float x2;
            float y1;
            float y2;

            RenderTexture.active = dest;
            bool  invertY = true; // source.texelSize.y < 0.0ff;
            // Set up the simple Matrix
            GL.PushMatrix();
            GL.LoadOrtho();

            for (int i = 0; i < material.passCount; i++)
            {
                material.SetPass(i);

                float y1_; float y2_;
                if (invertY)
                {
                    y1_ = 1.0f; y2_ = 0.0f;
                }
                else
                {
                    y1_ = 0.0f; y2_ = 1.0f;
                }

                // left
                x1 = 0.0f;
                x2 = 0.0f + 1.0f/(dest.width*1.0f);
                y1 = 0.0f;
                y2 = 1.0f;
                GL.Begin(GL.QUADS);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // right
                x1 = 1.0f - 1.0f/(dest.width*1.0f);
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // top
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 0.0f + 1.0f/(dest.height*1.0f);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // bottom
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 1.0f - 1.0f/(dest.height*1.0f);
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                GL.End();
            }

            GL.PopMatrix();
        }
    }
}
