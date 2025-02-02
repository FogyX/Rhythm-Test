using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImageEffectSelector : MonoBehaviour
{
    public int materialIndex = -1;

    public Material[] materials;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (materials.Length > 0 && materialIndex >= 0 && materialIndex < materials.Length && materials[materialIndex] != null)
        {
            Graphics.Blit(src, dest, materials[materialIndex]);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
