// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using System.Collections.Generic;
using UnityEngine;

namespace LDG.SoundReactor
{
    //[DisallowMultipleComponent]
    public class MidiTrackColorDriver : PropertyDriver
    {
        //public ColorMode colorMode = ColorMode.Magnitude;
        //public bool stationaryToggle = false;
        public Gradient restingColor = new Gradient();
        public List<Gradient> trackColors;
        public int materialIndex = 0;

        private Material[] materials;
        private int colorID;
        private ParticleSystem ps;
        private VertexElementColor vertexColor;

        private void Start()
        {
            MeshRenderer meshRenderer;
            SpriteRenderer spriteRenderer;

            if ((meshRenderer = GetComponent<MeshRenderer>()))
            {
                if ((materials = meshRenderer.materials) != null)
                {
                    colorID = Shader.PropertyToID("_Color");
                }
            }

            if ((spriteRenderer = GetComponent<SpriteRenderer>()))
            {
                if ((materials = spriteRenderer.materials) != null)
                {
                    colorID = Shader.PropertyToID("_Color");
                }
            }

            ps = GetComponent<ParticleSystem>();

            vertexColor = GetComponent<VertexElementColor>();
            
            if (!ps && !vertexColor && (!meshRenderer && materials == null))
            {
                //Debug.LogWarning("ColorDriver can't find a material, particle system, or vertex color.", this);
                componentMissing = true;
            }
        }

        public MidiTrackColorDriver()
        {
            SetColorSpectrum();
        }

        private Gradient ColorsToGradient(Color[] colors)
        {
            Gradient gradient = new Gradient();
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

            return gradient;
        }

        public void SetColorSpectrum()
        {
            trackColors = new List<Gradient>()
            {
               ColorsToGradient(new Color[] { Color.red }), // red
               ColorsToGradient(new Color[] { new Color(245f/255f, 0f/255f, 170f/255f, 1) }), // magenta
               ColorsToGradient(new Color[] { new Color(0f/255f, 50f/255f, 195f/255f, 1) }), // blue
               ColorsToGradient(new Color[] { new Color(0f/255f, 195f/255f, 250f/255f, 1) }), // light blue
               ColorsToGradient(new Color[] { new Color(0f/255f, 255f/255f, 65f/255f, 1) }), // green
               ColorsToGradient(new Color[] { new Color(230f/255f, 255f/255f, 0f/255f, 1) }), // yellow
               ColorsToGradient(new Color[] { new Color(255f/255f, 79f/255f, 0, 1) }), // orange
            };
        }

        protected override void DoLevel()
        {
            MidiTrackColorDriver colorDriver = (sharedDriver != null) ? (MidiTrackColorDriver)sharedDriver : this;

            int i = 0;
            Material material = null;

            SoundReactor.Note note = GetMidiNote();

            if (!ps && !vertexColor)
            {
                i = Mathf.Min(colorDriver.materialIndex, materials.Length - 1);
                material = materials[i];
            }

            float level = LevelScalar();
            int trackIndex = note.TrackIndex % colorDriver.trackColors.Count;


            Gradient gradient = colorDriver.trackColors[trackIndex];
            Color mainColor = Color.white;
            Color restingColor = Color.black;

            restingColor = colorDriver.restingColor.Evaluate(level);

            if (level < 0.005f)
            {
                mainColor = restingColor;
            }
            else
            {
                mainColor = gradient.Evaluate(level);
            }

            // assign mainColor
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
