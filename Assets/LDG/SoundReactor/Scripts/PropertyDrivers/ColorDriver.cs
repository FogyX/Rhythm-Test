// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.SoundReactor
{
    [System.Serializable]
    public enum ColorMode { Magnitude, Frequency };

    //[DisallowMultipleComponent]
    public class ColorDriver : PropertyDriver
    {
        private Material[] materials;
        private int colorID;
        private ParticleSystem ps;
        private VertexElementColor vertexColor;

        public ColorMode colorMode = ColorMode.Magnitude;
        public bool stationaryToggle = false;
        public Gradient mainColor = new Gradient();
        public Gradient restingColor = new Gradient();
        public int materialIndex = 0;

        private void OnDestroy()
        {
            if (materials != null)
            {
                foreach (Material mat in materials)
                {
                    Destroy(mat);
                }
            }
        }

        private void Awake()
        {
            MeshRenderer meshRenderer;
            SpriteRenderer spriteRenderer;

            colorID = Shader.PropertyToID("_Color");

            if ((meshRenderer = GetComponent<MeshRenderer>()))
            {
                materials = meshRenderer.materials;
            }

            if ((spriteRenderer = GetComponent<SpriteRenderer>()))
            {
                materials = spriteRenderer.materials;
            }

            ps = GetComponent<ParticleSystem>();

            vertexColor = GetComponent<VertexElementColor>();
            
            if (!ps && !vertexColor && (!meshRenderer && materials == null))
            {
                //Debug.LogWarning("ColorDriver can't find a material, particle system, or vertex color.", this);
                componentMissing = true;
            }
        }

        public ColorDriver()
        {
            SetColorDefault();
        }

        private void ColorsToGradient(Gradient gradient, Color[] colors)
        {
            float time;

            GradientColorKey[] colorKeys = new GradientColorKey[colors.Length];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                time = (float)i / (float)(colors.Length - 1);

                colorKeys[i].color = colors[i];
                alphaKeys[i].alpha = colors[i].a;

                colorKeys[i].time = alphaKeys[i].time = time;
            }

            gradient.SetKeys(colorKeys, alphaKeys);
        }

        public void SetColorSpectrum()
        {
            ColorsToGradient(mainColor, new Color[] { Color.red, new Color(1.0f, 0.5f, 0.0f), Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta });
        }

        public void SetColorDefault()
        {
            Color[] colors = new Color[] { new Color(0.0f, 0.5f, 1.0f), Color.magenta };

            ColorsToGradient(mainColor, colors);
            ColorsToGradient(restingColor, colors);
        }

        protected override void DoLevel()
        {
            ColorDriver colorDriver = (sharedDriver != null) ? (ColorDriver)sharedDriver : this;

            int i = 0;
            Material material = null;
            float v = 0.0f;

            if (!ps && !vertexColor)
            {
                i = Mathf.Min(colorDriver.materialIndex, materials.Length - 1);
                material = materials[i];
            }

            float level = LevelScalar();

            Color mainColor = new Color(0.0f, 0.5f, 1.0f);
            Color restingColor = Color.magenta;

            if (colorDriver.stationaryToggle && !vertexColor)
            {
                if (colorDriver.colorMode == ColorMode.Frequency)
                {
                    v = this.level.linearizedFrequency;
                }
                else
                {
                    v = this.level.normalizedLevel;
                }

                if (level >= this.level.normalizedLevel)
                {
                    mainColor = Color.Lerp(colorDriver.restingColor.Evaluate(v), colorDriver.mainColor.Evaluate(v), level);
                }
                else
                {
                    mainColor = colorDriver.restingColor.Evaluate(v);
                }
            }
            else
            {
                switch (colorDriver.colorMode)
                {
                    case ColorMode.Magnitude:
                        mainColor = colorDriver.mainColor.Evaluate(level);
                        restingColor = colorDriver.restingColor.Evaluate(level);

                        break;

                    case ColorMode.Frequency:
                        v = base.level.linearizedFrequency;

                        if (vertexColor)
                        {
                            mainColor = colorDriver.mainColor.Evaluate(v);
                            restingColor = colorDriver.restingColor.Evaluate(v);
                        }
                        else
                        {
                            mainColor = Color.Lerp(colorDriver.restingColor.Evaluate(v), colorDriver.mainColor.Evaluate(v), level);
                        }

                        break;
                }
            }

            if (ps)
            {
                ParticleSystem.MainModule module = ps.main;
                module.startColor = mainColor;
            }
            else if (material)
            {
                material.SetColor(colorID, mainColor);
            }
            else if(vertexColor)
            {
                vertexColor.mainColor = mainColor;
                vertexColor.restingColor = restingColor;
            }
        }
    }
}
