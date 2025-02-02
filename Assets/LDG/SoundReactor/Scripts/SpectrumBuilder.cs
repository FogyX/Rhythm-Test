// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LDG.SoundReactor
{
    public enum ShapeMode { Line, Circle, Rectangle, SegmentedLevels }
    public enum VectorShapeMode { Line, Circle }
    public enum SegmentMode { Object, Vector, Piano }
    public enum SpacingMode { Spaced, Divided }
    public enum BuilderNote { C, CS, D, DS, E, F, FS, G, GS, A, AS, B}

    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SpectrumBuilder : SerializeableObject
    {
        #region Events
        public event Action<SpectrumBuilder> OnBuildSpectrum;
        #endregion
        #region Properties
        public float spacing
        {
            get
            {
                return CalcColumnSpacing();
            }
        }

        public int firstKey
        {
            get
            {
                Frequency.SetBaseFrequency(FrequencyBase.Midi);
                return Mathf.RoundToInt(Frequency.LinearizeFrequency(frequencyLower) * 127.0f);
            }
        }

        public int lastKey
        {
            get
            {
                Frequency.SetBaseFrequency(FrequencyBase.Midi);
                return Mathf.RoundToInt(Frequency.LinearizeFrequency(frequencyUpper) * 127.0f);
            }
        }
        #endregion
        #region Fields
        public ShapeMode shape = ShapeMode.Line;
        public SegmentMode segmentMode = SegmentMode.Object;
        public int numColumns = 7;
        public int numRows = 10;
        public Texture2D texture;

        public SpacingMode spacingMode;
        public bool fit = true;
        public bool betweenEdges = true;
        public bool fromTexture = true;
		
        public GameObject levelInstance;
        public List<GameObject> levelInstanceList = new List<GameObject>();

        static string[] noteNames = new string[] { "C", "CS", "D", "DS", "E", "F", "FS", "G", "GS", "A", "AS", "B" };
        static string[] noteStartNames = new string[] { "C", "CS", "D_Start", "DS", "E_Start", "F", "FS", "G_Start", "GS", "A_Start", "AS", "B_Start" };
        static string[] noteEndNames = new string[] { "C_End", "CS", "D_End", "DS", "E", "F_End", "FS", "G_End", "GS", "A_End", "AS", "B" };
        public GameObject pianoKeys;
        
        public bool shareDriver = true;
        public Vector2 layoutSize = new Vector2(10, 10);
        public Vector3 levelSize = Vector3.one;
        public Vector2 levelSpacing = new Vector2(0.1f, 0.1f);
        public float travel = 1.0f;

        public FrequencyBase frequencyMode;
        public FrequencyRangeOption frequencyRangeOption = FrequencyRangeOption.FullRange;
        public MidiRangeOption midiRangeOption = MidiRangeOption.Key88;
        public float frequencyLower = 20.0f;
        public float frequencyUpper = 20000.0f;

        public float transformRepeat = 1.0f;
        public bool transformAlternate = false;
        public bool transformReverse = false;
        public bool transformFlipLevel = false;
        public bool clamp = true;

        public VectorShape vectorShape = null;
        public bool vectorAnchored = false;
        public float vectorAnchoredDiameter = 0.0f;
        public bool closeCurve = false;
        public ColorDriver colorDriver;
        public Material vectorMaterial;

        public bool autoBuild = false;
        #endregion

        public void Awake()
        {
            // for backwards compatability
            if (segmentMode == SegmentMode.Object)
            {
                if (levelInstanceList.Count < 1)
                {
                    levelInstanceList.Add(this.levelInstance);
                }

                if (levelInstanceList[0] == null) return;
            }
        }

        /// <summary>
        /// Unity Instantiate variant.
        /// </summary>
        public GameObject Instantiate(GameObject original, Transform parent)
        {
            GameObject go;
            
#if UNITY_EDITOR
            if((go = (GameObject)PrefabUtility.InstantiatePrefab(original)) == null)
            {
#if UNITY_4_6
				go = (GameObject)Instantiate(original);
#else
                go = Instantiate<GameObject>(original);
#endif
            }
#else
            go = Instantiate<GameObject>(original);
#endif

            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;

            return go;
        }

        /// <summary>
        /// Sets the level info for a level at a given index.
        /// </summary>
        void SetLevelInfo(GameObject levelObject, GameObject sharedDriver, int arraySize, int index)
        {
            float linearizedFrequency = (float)index / (float)(arraySize - 1);

            SetLevelInfo(levelObject, sharedDriver, linearizedFrequency, 0.0f);
        }

        /// <summary>
        /// Sets the level info for a level at a given index.
        /// </summary>
        void SetLevelInfo(GameObject levelObject, GameObject sharedDriver, int arraySize, int index, float normalizedLevel)
        {
            float normalizedIndex = (float)index / (float)(arraySize - 1);

            SetLevelInfo(levelObject, sharedDriver, normalizedIndex, normalizedLevel);
        }

        /// <summary>
        /// Sets the level info for a level at a given linear frequency (linear frequency is normalized).
        /// </summary>
        void SetLevelInfo(GameObject levelObject, GameObject sharedDriver, float normalizedIndex, float normalizedLevel)
        {
            Level level;

            if ((level = levelObject.GetComponent<Level>()) == null)
            {
                level = levelObject.AddComponent<Level>();
            }
            
            if (level)
            {
                level.Set(normalizedIndex, normalizedLevel, frequencyMode, frequencyLower, frequencyUpper, new FrequencyTransform(transformRepeat, clamp, transformAlternate, transformReverse, transformFlipLevel));
            }

            if (shareDriver && sharedDriver)
            {
                AttachSharedDriver(levelObject, sharedDriver);
            }
        }

        /// <summary>
        /// Attaches a shared object to all the levels built.
        /// </summary>
        void AttachSharedDriver(GameObject levelObject, GameObject sharedObject)
        {
            Transform instanceChild;

            PropertyDriver[] sharedDriver;
            PropertyDriver[] levelDriver;

            sharedDriver = sharedObject.GetComponents<PropertyDriver>();
            levelDriver = levelObject.GetComponents<PropertyDriver>();

            for (int i = 0; i < sharedDriver.Length; i++)
            {
                levelDriver[i].sharedDriver = sharedDriver[i];
            }

            foreach (Transform segmentChild in levelObject.transform)
            {
                instanceChild = sharedObject.transform.Find(segmentChild.name);

                AttachSharedDriver(segmentChild.gameObject, instanceChild.gameObject);
            }
        }

        /// <summary>
        /// Deletes all the levels that were built
        /// </summary>
        void DeleteLevels()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Calculates column spacing based on a space, or divisions inside a specific size, and whether they are centered
        /// on the width, or fit inside the width.
        /// </summary>
        private float CalcColumnSpacing()
        {
            float spacing = 0.0f;
            float size = 0.0f;

            switch (spacingMode)
            {
                case SpacingMode.Spaced:
                    size = betweenEdges ? levelSize.x : 0.0f;

                    spacing = size + levelSpacing.x;
                    break;

                case SpacingMode.Divided:
                    size = fit ? levelSize.x : 0.0f;

                    spacing = (layoutSize.x - size) / (numColumns - 1);
                    break;
            }

            return spacing;
        }

        /// <summary>
        /// Calculates row spacing based on a space, or divisions inside a specific size, and whether they are centered
        /// on the height, or fit inside the height.
        /// </summary>
        private float CalcRowSpacing()
        {
            float spacing = 0.0f;
            float size = 0.0f;

            switch (spacingMode)
            {
                case SpacingMode.Spaced:
                    size = betweenEdges ? levelSize.y : 0.0f;

                    spacing = size + levelSpacing.y;
                    break;

                case SpacingMode.Divided:
                    size = fit ? levelSize.y : 0.0f;

                    spacing = (layoutSize.y - size) / (numRows - 1);
                    break;
            }

            return spacing;
        }
        
        /// <summary>
        /// Builds a vector line
        /// </summary>
        public void BuildVectorLine()
        {
            if (!colorDriver || !vectorMaterial) return;

            DeleteLevels();

            Transform levelTransform;

            float halfSize;
            float spacing = CalcColumnSpacing();

            halfSize = spacing * (numColumns - 1) * 0.5f;

            for (int i = 0; i < numColumns; i++)
            {
                levelTransform = new GameObject().transform;
                levelTransform.name = "Level" + i.ToString();

                Vector3 pos = levelTransform.localPosition;
                pos.x = -halfSize + i * spacing;

                levelTransform.parent = transform;
                levelTransform.localPosition = pos;
                levelTransform.localScale = levelSize;

                levelTransform.gameObject.AddComponent<Level>();

                SetLevelInfo(levelTransform.gameObject, null, numColumns, i);
            }

            vectorShape = new VectorShape(colorDriver, transform, true);
        }

        /// <summary>
        /// Builds a vector circle
        /// </summary>
        public void BuildVectorCircle()
        {
            if (!colorDriver || !vectorMaterial) return;

            DeleteLevels();

            int nVertices = numColumns + 1;
            Transform levelTransform;

            float spacing = CalcColumnSpacing();
            float radius;
            float arc = (Mathf.PI * 2.0f) / (float)(nVertices - 1);

            if (spacingMode == SpacingMode.Spaced)
            {
                radius = spacing * (float)nVertices / Mathf.PI * 0.5f;
            }
            else
            {
                radius = layoutSize.x * 0.5f;
            }

            if (fit && spacingMode == SpacingMode.Divided)
            {
                radius -= levelSize.y * 0.5f;
            }

            for (int i = 0; i < nVertices; i++)
            {
                levelTransform = new GameObject().transform;
                levelTransform.name = "Level" + i.ToString();

                Vector3 pos = levelTransform.localPosition;
                pos.x = Mathf.Cos(i * arc + Mathf.PI * 0.5f) * radius;
                pos.y = Mathf.Sin(i * arc + Mathf.PI * 0.5f) * radius;

                levelTransform.parent = transform;
                levelTransform.localRotation = Quaternion.AngleAxis((arc * i) * Mathf.Rad2Deg, Vector3.forward);
                levelTransform.localPosition = pos;
                levelTransform.localScale = levelSize;

                levelTransform.gameObject.AddComponent<Level>();

                SetLevelInfo(levelTransform.gameObject, null, nVertices, (nVertices - 1) - i);
            }

            vectorShape = new VectorShape(colorDriver, transform, false);
        }

        /// <summary>
        /// Builds a line made up of GameObjects
        /// </summary>
        public void BuildObjectLine()
        {
            if (levelInstanceList.Count == 0) return;

            DeleteLevels();

            Transform levelTransform;

            float spacing;
            float halfSize;
            int levelIndex;
            
            spacing = CalcColumnSpacing();
            halfSize = numColumns * spacing / 2 - spacing / 2;

            for (int i = 0; i < numColumns; i++)
            {
                levelIndex = i % levelInstanceList.Count;
                
                // only add the levelInstance if it exists
                if (levelInstanceList[levelIndex] != null)
                {
                    levelInstance = levelInstanceList[levelIndex];

                    levelTransform = Instantiate(levelInstance, transform).transform;
                    levelTransform.name = "Level" + i.ToString();

                    Vector3 pos = levelTransform.localPosition;
                    pos.x = -halfSize + i * spacing;

                    levelTransform.localPosition = pos;
                    levelTransform.localScale = levelSize;

                    SetLevelInfo(levelTransform.gameObject, levelInstance, numColumns, i);
                }
            }
        }

        /// <summary>
        /// Builds an object line
        /// </summary>
        public void BuildPianoLine()
        {
            if (!pianoKeys) return;

            DeleteLevels();

            Transform levelTransform;
            GameObject pianoKey;

            float spacing;
            float halfSize;

            spacing = CalcColumnSpacing();
            halfSize = numColumns * spacing / 2 - spacing / 2;

            for (int i = 0; i < numColumns; i++)
            {
                if (i == 0)
                {
                    pianoKey = pianoKeys.transform.Find(noteStartNames[(i + firstKey) % 12]).gameObject;
                }
                else if(i == numColumns - 1)
                {
                    pianoKey = pianoKeys.transform.Find(noteEndNames[(i + firstKey) % 12]).gameObject;
                }
                else
                {
                    pianoKey = pianoKeys.transform.Find(noteNames[(i + firstKey) % 12]).gameObject;
                }

                if (pianoKey)
                {
                    levelTransform = Instantiate(pianoKey, transform).transform;
                    levelTransform.name = "Level" + i.ToString();

                    Vector3 pos = levelTransform.localPosition;
                    pos.x = -halfSize + i * spacing;

                    levelTransform.localPosition = pos;
                    levelTransform.localScale = levelSize;

                    SetLevelInfo(levelTransform.gameObject, pianoKey, numColumns, i);
                }
            }
        }

        public void BuildPianoCircle()
        {
            if (!pianoKeys) return;

            DeleteLevels();

            Transform levelTransform = gameObject.transform;
            GameObject pianoKey;
            float radius;

            float spacing = CalcColumnSpacing();
            float arc = 360.0f / (float)(numColumns - 1);

            if (spacingMode == SpacingMode.Spaced)
            {
                radius = spacing * (float)(numColumns) / Mathf.PI * 0.5f;
            }
            else
            {
                radius = layoutSize.x * 0.5f;
            }

            if (fit && spacingMode == SpacingMode.Divided)
            {
                radius -= levelSize.y * 0.5f;
            }

            // only add the levelInstance if it exists
            for (int i = 0; i < numColumns - 1; i++)
            {
                if (i == 0)
                {
                    pianoKey = pianoKeys.transform.Find(noteStartNames[(i + firstKey) % 12]).gameObject;
                }
                else if (i == numColumns - 1)
                {
                    pianoKey = pianoKeys.transform.Find(noteEndNames[(i + firstKey) % 12]).gameObject;
                }
                else
                {
                    pianoKey = pianoKeys.transform.Find(noteNames[(i + firstKey) % 12]).gameObject;
                }

                if (pianoKey)
                {
                    levelTransform = Instantiate(pianoKey, transform).transform;
                    levelTransform.name = "Level" + i.ToString();

                    levelTransform.localRotation = Quaternion.AngleAxis(arc * -i, Vector3.forward);
                    levelTransform.localPosition = transform.InverseTransformDirection(levelTransform.up) * radius;
                    levelTransform.localScale = levelSize;

                    SetLevelInfo(levelTransform.gameObject, levelInstance, numColumns, i);
                }
            }
        }

#if UNITY_EDITOR
        private bool TextureReadOnly(UnityEngine.Object texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
            
            return !textureImporter.isReadable;
        }
#endif

        /// <summary>
        /// Builds up levels from a specified image.
        /// </summary>
        public void BuildObjectRectangle()
        {
            if (levelInstanceList.Count == 0) return;

            DeleteLevels();

            Transform levelTransform;

            if (fromTexture)
            {
                if (texture == null)
                {
                    Debug.Log("Missing texture");
                    return;
                }

#if UNITY_EDITOR
                if (TextureReadOnly(texture))
                {
                    Debug.LogWarning("Read/Write must be enabled on the texture");
                    return;
                }
#endif

                numColumns = texture.width;
                numRows = texture.height;
            }
            
            float columnSpacing = CalcColumnSpacing();
            float rowSpacing = CalcRowSpacing();

            float halfWidth = columnSpacing * numColumns / 2.0f - columnSpacing * 0.5f;
            float halfHeight = rowSpacing * numRows / 2.0f - rowSpacing * 0.5f;
            int nameIndex = 0;
            int levelIndex;
            bool create = true;
            float normalizedIndex = 1.0f;

            for (int y = numRows - 1; y >= 0; y--)
            {
                for (int x = 0; x < numColumns; x++)
                {
                    if(fromTexture)
                    {
                        Color c = texture.GetPixel(x, y);
                        create = c.a != 0.0f;
                        normalizedIndex = c.r;
                    }

                    if (create)
                    {
                        levelIndex = nameIndex % levelInstanceList.Count;

                        // only add the levelInstance if it exists
                        if (levelInstanceList[levelIndex] != null)
                        {
                            levelInstance = levelInstanceList[levelIndex];
                            levelTransform = Instantiate(levelInstance, transform).transform;
                            levelTransform.name = "Level" + nameIndex.ToString();

                            Vector3 pos = new Vector3((float)x * columnSpacing - halfWidth, (float)y * rowSpacing - halfHeight, 0.0f);
                            levelTransform.localPosition = pos;
                            levelTransform.localScale = levelSize;

                            if (fromTexture)
                            {
                                SetLevelInfo(levelTransform.gameObject, levelInstance, normalizedIndex, 0.0f);
                            }
                            else
                            {
                                SetLevelInfo(levelTransform.gameObject, levelInstance, numColumns * numRows, nameIndex);
                            }

                            nameIndex++;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Builds up levels as segments. Each column is a frequency, and the rows are the magnitudes.
        /// </summary>
        public void BuildObjectSegmentedLevels()
        {
            if (levelInstanceList.Count == 0) return;

            DeleteLevels();

            Transform levelTransform;
            
            float columnSpacing = CalcColumnSpacing();
            float rowSpacing = CalcRowSpacing();

            float halfWidth = columnSpacing * numColumns / 2.0f - columnSpacing * 0.5f;
            float halfHeight = rowSpacing * numRows / 2.0f - rowSpacing * 0.5f;

            int nameIndex = 0;
            int levelIndex;

            for (int x = 0; x < numColumns; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    levelIndex = nameIndex % levelInstanceList.Count;

                    // only add the levelInstance if it exists
                    if (levelInstanceList[levelIndex] != null)
                    {
                        levelInstance = levelInstanceList[levelIndex];

                        levelTransform = Instantiate(levelInstance, transform).transform;
                        levelTransform.name = "Level" + nameIndex.ToString();

                        Vector3 pos = new Vector3((float)x * columnSpacing - halfWidth, (float)y * rowSpacing - halfHeight, 0.0f);
                        levelTransform.localPosition = pos;
                        levelTransform.localScale = levelSize;

                        float normalizedLevel = (float)y / (float)(numRows - 1);

                        SetLevelInfo(levelTransform.gameObject, levelInstance, numColumns, x, normalizedLevel);
                    }

                    nameIndex++;
                }
            }
        }
        

        public void BuildObjectCircle()
        {
            if (levelInstanceList.Count == 0) return;

            DeleteLevels();

            int levelIndex;
            Transform levelTransform = gameObject.transform;
            float radius;

            float spacing = CalcColumnSpacing();
            float arc = 360.0f / (float)(numColumns - 1);

            if (spacingMode == SpacingMode.Spaced)
            {
                radius = spacing * (float)(numColumns) / Mathf.PI * 0.5f;
            }
            else
            {
                radius = layoutSize.x * 0.5f;
            }

            if(fit && spacingMode == SpacingMode.Divided)
            {
                radius -= levelSize.y * 0.5f;
            }

            // only add the levelInstance if it exists
            for (int i = 0; i < numColumns - 1; i++)
            {
                levelIndex = i % levelInstanceList.Count;

                // only add the levelInstance if it exists
                if (levelInstanceList[levelIndex] != null)
                {
                    levelInstance = levelInstanceList[levelIndex];

                    levelTransform = Instantiate(levelInstance, transform).transform;
                    levelTransform.name = "Level" + i.ToString();

                    levelTransform.localRotation = Quaternion.AngleAxis(arc * -i, Vector3.forward);
                    levelTransform.localPosition = transform.InverseTransformDirection(levelTransform.up) * radius;
                    levelTransform.localScale = levelSize;

                    SetLevelInfo(levelTransform.gameObject, levelInstance, numColumns, i);
                }
            }
        }

        /// <summary>
        /// Builds a shape given the current settings for the builder.
        /// </summary>
        public void Build()
        {
            if (segmentMode == SegmentMode.Object)
            {
                // backwards compatibility. sound reactor only uses a list of levels now, so add legacy level to list.
                if (levelInstanceList.Count == 0 && this.levelInstance != null)
                {
                    levelInstanceList.Add(this.levelInstance);
                }

                if(levelInstanceList.Count == 0)
                {
                    Debug.Log("Cannot build spectrum: Missing Level(s)");
                    return;
                }

                foreach(GameObject go in levelInstanceList)
                {
                    if(go == null)
                    {
                        Debug.Log("Cannot build spectrum: Missing Level(s)");
                        return;
                    }
                }
            }
            
            switch (shape)
            {
                case ShapeMode.Line:
                    {
                        switch (segmentMode)
                        {
                            case SegmentMode.Vector:
                                BuildVectorLine();
                                break;

                            case SegmentMode.Object:
                                BuildObjectLine();
                                break;

                            case SegmentMode.Piano:
                                BuildPianoLine();
                                break;
                        }

                        break;
                    }

                case ShapeMode.Circle:
                    {
                        switch (segmentMode)
                        {
                            case SegmentMode.Vector:
                                BuildVectorCircle();
                                break;

                            case SegmentMode.Object:
                                BuildObjectCircle();
                                break;

                            case SegmentMode.Piano:
                                BuildPianoCircle();
                                break;
                        }

                        break;
                    }

                case ShapeMode.Rectangle:
                    {
                        BuildObjectRectangle();
                        break;
                    }

                case ShapeMode.SegmentedLevels:
                    {
                        BuildObjectSegmentedLevels();
                        break;
                    }
            }

            if (OnBuildSpectrum != null)
            {
                OnBuildSpectrum(this);
            }
        }

        private void Start()
        {
            vectorShape = null;
        }

        /// <summary>
        /// If the builder created a vector, then the vector is handled/drawn here.
        /// </summary>
        void OnRenderObject()
        {
            // don't draw this in the preview window
            if (Camera.current != null && Camera.current.cameraType == CameraType.Preview) return;

            // convert layer bit to int
            int layerInt = 1 << gameObject.layer;

            if (Camera.current != null && (Camera.current.cullingMask & layerInt) == layerInt)
            {
                if (segmentMode == SegmentMode.Vector)
                {
                    if (vectorShape == null)
                    {
                        if (shape == ShapeMode.Line)
                        {
                            vectorShape = new VectorShape(colorDriver, transform, true);
                        }
                        else if (shape == ShapeMode.Circle)
                        {
                            vectorShape = new VectorShape(colorDriver, transform, false);
                        }
                    }
                    else
                    {
                        if (vectorMaterial)
                        {
                            if (shape == ShapeMode.Line)
                            {
                                vectorShape.Draw(transform, layoutSize.x * 0.5f, levelSize.y, travel, true, vectorAnchored, vectorAnchoredDiameter, vectorMaterial);
                            }
                            else if (shape == ShapeMode.Circle)
                            {
                                vectorShape.Draw(transform, layoutSize.x * 0.5f, levelSize.y, travel, false, vectorAnchored, vectorAnchoredDiameter, vectorMaterial);
                            }
                        }
                    }
                }
            }
        }
    }

}