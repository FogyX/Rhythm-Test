using UnityEngine;
using LDG.SoundReactor;

public class EmissionHandler : MonoBehaviour
{
    public Color emissionColor;

    private Material material;
    private int emissionColorID;

    private void Start()
    {
        MeshRenderer meshRenderer;

        if ((meshRenderer = GetComponent<MeshRenderer>()))
        {
            if ((material = meshRenderer.material) != null)
            {
                emissionColorID = Shader.PropertyToID("_EmissionColor");
            }
        }
    }

    public void OnLevel(PropertyDriver driver)
    {
        if (material)
        {
            float level = driver.LevelScalar();

            material.SetColor(emissionColorID, emissionColor * level);
        }
    }
}