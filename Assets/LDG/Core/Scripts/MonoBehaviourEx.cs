// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System;
using System.Collections;
using UnityEngine;

namespace LDG.Core
{
    /// <summary>
    /// Methods to help ease changing component properties. These are slow methods and should not be used many time per frame.
    /// If more frequent changes are intented, components should be cached and its properties changed that way.
    /// </summary>
    public class MonoBehaviourEx : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private MeshRenderer meshRenderer
        {
            get
            {
                if(!_meshRenderer)
                {
                    _meshRenderer = GetComponent<MeshRenderer>();
                }

                return _meshRenderer;
            }
        }

        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer spriteRenderer
        {
            get
            {
                if (!_spriteRenderer)
                {
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                }

                return _spriteRenderer;
            }
        }

        private MaterialPropertyBlock _propertyBlock;
        protected MaterialPropertyBlock propertyBlock
        {
            get
            {
                if (_propertyBlock == null)
                {
                    _propertyBlock = new MaterialPropertyBlock();
                }

                return _propertyBlock;
            }
        }

        /// <summary>
        /// Destroys all the children under this transform. Optionally set to true to destroy immediately.
        /// </summary>
        public void ClearChildren(bool immediate = false)
        {
            int childCount = transform.childCount;

            if (immediate)
            {
                for (int i = 0; i < childCount; i++)
                {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
            else
            {
                for (int i = 0; i < childCount; i++)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
            }
        }

        /// <summary>
        /// Gets the default "_Color" from a material. Returns MAGENTA if a MeshRenderer is not attached to the GameObject. Call infrequently per frame.
        /// </summary>
        public Color GetMaterialColor()
        {
            Color color = Color.magenta;

            if (meshRenderer && meshRenderer.sharedMaterial)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
#if UNITY_2017_3_OR_NEWER
                color = propertyBlock.GetColor("_Color");
#else
                color = propertyBlock.GetVector("_Color");
#endif
                meshRenderer.SetPropertyBlock(propertyBlock);
            }

            return color;
        }

        /// <summary>
        /// Sets color of the "_Color" property of a material. Will not assign a color if the material doesn't have a "_Color" property, or if
        /// a MeshRenderer is not attached to the GameObject. Call infrequently per frame.
        /// </summary>
        public bool SetMaterialColor(Color color)
        {
            if (meshRenderer && meshRenderer.sharedMaterial)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", color);
                meshRenderer.SetPropertyBlock(propertyBlock);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets color of the "_Color" property of a material. Will not assign a color if the material doesn't have a "_Color" property, or if
        /// a MeshRenderer is not attached to the GameObject. Call infrequently per frame.
        /// </summary>
        public bool SetSpriteMaterialColor(Color color)
        {
            if (spriteRenderer && spriteRenderer.sharedMaterial)
            {
                spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", color);
                spriteRenderer.SetPropertyBlock(propertyBlock);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the color of the material by property name. Returns MAGENTA if a MeshRenderer is not attached to the GameObject. Call
        /// infrequently per frame.
        /// </summary>
        /// <param name="name">Name of the color property used by the shader attached to the material.</param>
        public Color GetMaterialColor(string name)
        {
            Color color = Color.magenta;

            if (meshRenderer && meshRenderer.sharedMaterial)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
#if UNITY_2017_3_OR_NEWER
                color = propertyBlock.GetColor(name);
#else
                color = propertyBlock.GetVector(name);
#endif
                meshRenderer.SetPropertyBlock(propertyBlock);
            }

            return color;
        }

        /// <summary>
        /// Sets color of a material by property name. Will not assign a color if the material doesn't have the specified color property, or if
        /// a MeshRenderer is not attached to the GameObject. Call infrequently per frame.
        /// </summary>
        /// <param name="name">Name of the color property used by the shader attached to the material.</param>
        /// <returns>True if the color was set</returns>
        public bool SetMaterialColor(string name, Color color)
        {
            if (meshRenderer && meshRenderer.sharedMaterial)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(name, color);
                meshRenderer.SetPropertyBlock(propertyBlock);

                return true;
            }
            else
            {
                Debug.LogWarning("Either a MeshRenderer or material is missing from this GameObject", gameObject);
            }

            return false;
        }

        /// <summary>
        /// Sets color of a material by property name. Will not assign a color if the material doesn't have the specified color property, or if
        /// a MeshRenderer is not attached to the GameObject. Call infrequently per frame.
        /// </summary>
        /// <param name="name">Name of the color property used by the shader attached to the material.</param>
        /// <returns>True if the color was set</returns>
        public bool SetMaterialFloat(string name, float value)
        {
            if (meshRenderer && meshRenderer.sharedMaterial)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(name, value);
                meshRenderer.SetPropertyBlock(propertyBlock);

                return true;
            }
            else
            {
                Debug.LogWarning("Either a MeshRenderer or material is missing from this GameObject", gameObject);
            }

            return false;
        }

        public void EnableMeshRenderer(bool enabled)
        {
            if (meshRenderer)
            {
                meshRenderer.enabled = enabled;
            }
            else
            {
                Debug.LogWarning("There's no MeshRenderer to disable on this GameObject", gameObject);
            }
        }

        public void EnableGameObject()
        {
            gameObject.SetActive(true);
        }

        IEnumerator DisableGameObjectEndOfFrameYield()
        {
            yield return new WaitForEndOfFrame();

            gameObject.SetActive(false);
        }

        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Disables the game object once the current frame is done. This ensures that any scripts attached to this object
        /// will call awake first.
        /// </summary>
        public void DisableGameObjectEndOfFrame()
        {
            StartCoroutine(DisableGameObjectEndOfFrameYield());
        }

        public void DisableGameObjectBuild()
        {
#if !UNITY_EDITOR
            gameObject.SetActive(false);
#endif
        }

        public void DisableGameObjectReleaseBuild()
        {
            if (!Debug.isDebugBuild)
            {
#if !UNITY_EDITOR
                gameObject.SetActive(false);
#endif
            }
        }

        public void DisableGameObjectEndOfFrameBuild()
        {
#if !UNITY_EDITOR
        StartCoroutine(DisableGameObjectEndOfFrameYield());
#endif
        }

        public void EnableMeshRenderer()
        {
            EnableMeshRenderer(true);
        }

        public void DisableMeshRenderer()
        {
            EnableMeshRenderer(false);
        }

        public void DestroyGameObject()
        {
            Destroy(gameObject);
        }

        public void DestroyGameObjectBuild()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
#endif
        }

        public void DestroyGameObjectReleaseBuild()
        {
            if (!Debug.isDebugBuild)
            {
#if !UNITY_EDITOR
                Destroy(gameObject);
#endif
            }
        }

        protected IEnumerator WaitForFrames(int waitFrames)
        {
            int finalFrame = Time.frameCount + waitFrames;

            for (int i = Time.frameCount; i < finalFrame; i++)
            {
                yield return null;
            }
        }
    }
}
