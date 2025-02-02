using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDG.SoundReactor
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(SpectrumBuilder))]
    [ExecuteInEditMode]
    public class SpectrumLineRenderer : SerializeableObject
    {
        public enum TravelDirection { X, Y, Z }

        public float travel = 8;
        public TravelDirection travelDirection = TravelDirection.Y;
        public bool alternateDirection = false;
        public int divisions = 10;

        private LineRenderer lineRenderer;
        private SpectrumBuilder spectrumBuilder;

        private Level[] levelsCache;
        private Vector3[] positions;

        // Start is called before the first frame update
        void Start()
        {
            if (!ValidateLevels())
            {
                Debug.LogError("SpectrumLineRenderer: Can't find Level component. All child objects must have a Level script attached to it.", gameObject);
                this.enabled = false;
                return;
            }

            lineRenderer = gameObject.GetComponent<LineRenderer>();

            // allocate data for spectrum
            UpdateSpectrum();
        }

        private void OnEnable()
        {
            spectrumBuilder = gameObject.GetComponent<SpectrumBuilder>();

            // used to update levels/positions whenever the builder is updated
            spectrumBuilder.OnBuildSpectrum += OnBuildSpectrum;
        }

        private void OnDisable()
        {
            spectrumBuilder = gameObject.GetComponent<SpectrumBuilder>();

            // used to update levels/positions whenever the builder is updated
            spectrumBuilder.OnBuildSpectrum -= OnBuildSpectrum;
        }

        // Update is called once per frame
        void Update()
        {
            // update the renderer, but if it fails, jump out
            if(!UpdateArrays()) return;

            int positionCount = lineRenderer.positionCount;
            float alternateDir = 1;

            for (int i = 0; i < levelsCache.Length; i++)
            {
                positions[i] = levelsCache[i].transform.position;

                if (alternateDirection)
                {
                    alternateDir = (i % 2 == 1) ? 1 : -1;
                }

                switch (travelDirection)
                {
                    case TravelDirection.X:
                        positions[i] += levelsCache[i].transform.right * levelsCache[i].fallingLevel * travel * alternateDir;
                        break;
                    case TravelDirection.Y:
                        positions[i] += levelsCache[i].transform.up * levelsCache[i].fallingLevel * travel * alternateDir;
                        break;
                    case TravelDirection.Z:
                        positions[i] += levelsCache[i].transform.forward * levelsCache[i].fallingLevel * travel * alternateDir;
                        break;
                }
            }

            for (int i = 0; i < positionCount; i++)
            {
                lineRenderer.SetPosition(i, Spline.Tween((float)i / (float)(positionCount - 1), positions, false, InterpolationMode.Curve));
            }
        }

        void OnBuildSpectrum(SpectrumBuilder sender)
        {
            UpdateSpectrum();
        }

        bool ValidateLevels()
        {
            int levelCount = 0;

            foreach (Transform child in transform)
            {
                if (child.GetComponent<Level>() != null) levelCount++;
            }

            // return false if any of the children don't have a level script attached to it
            return (levelCount == transform.childCount);
        }

        void UpdateSpectrum()
        {
            // jump out if there aren't any children because we can't allocate arrays to nothing
            if (transform.childCount < 1) return;

            lineRenderer.positionCount = transform.childCount * divisions;

            // allocate arrays for cache and line renderer positions
            levelsCache = new Level[transform.childCount];
            positions = new Vector3[transform.childCount];

            // cache levels
            for (int i = 0; i < transform.childCount; i++)
            {
                levelsCache[i] = transform.GetChild(i).GetComponent<Level>();
            }
        }

        bool UpdateArrays()
        {
            // jump out if there aren't any levels
            if(transform.childCount < 2) return false;

            divisions = Mathf.Clamp(divisions, 1, 20);

            // update the number of position used by the line renderer if something changed
            if (lineRenderer.positionCount != transform.childCount * divisions)
            {
                lineRenderer.positionCount = transform.childCount * divisions;
            }

            if(levelsCache == null)
            {
                UpdateSpectrum();
            }

            return true;
        }
    }
}