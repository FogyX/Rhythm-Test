// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;
using System.IO;

namespace LDG.SoundReactor
{
    public static class MenuItems
    {
        #region Helpers
        static void ParentAndUndo(GameObject gameObject, MenuCommand menuCommand)
        {
            GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeObject = gameObject;
        }

        static void ParentAndUndo(GameObject gameObject, GameObject parent)
        {
            GameObjectUtility.SetParentAndAlign(gameObject, parent);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeObject = gameObject;
        }
        #endregion
        
        #region Menu Asset

        [MenuItem("Assets/Create/SoundReactor/Midi Clip", false, 300)]
        static void MidiClipCreate()
        {
            MidiClip midiClip = ScriptableObject.CreateInstance<MidiClip>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            
            midiClip.midiFile = Selection.activeObject;

            midiClip.Read(path);

            ProjectWindowUtil.CreateAsset(midiClip, directory + "/" + fileName + ".midi.asset");
        }

        [MenuItem("Assets/Create/SoundReactor/Midi Clip", true)]
        static bool ValidateMidiClipCreate()
        {
            return MidiClip.Validate(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem("Tools/SoundReactor/Midi Clip", false, 111)]
        static void MidiClipAsset()
        {
            MidiClipCreate();
        }

        [MenuItem("Tools/SoundReactor/Midi Clip", true)]
        static bool ValidateMidiClipAsset()
        {
            return ValidateMidiClipCreate();
        }




        [MenuItem("Assets/Create/SoundReactor/Peaks Profile", false, 300)]
        static void PeaksProfileCreate()
        {
            PeaksProfile peaksProfile = ScriptableObject.CreateInstance<PeaksProfile>();

            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            FileAttributes attr = File.GetAttributes(assetPath);

            if (attr == FileAttributes.Directory)
            {
                ProjectWindowUtil.CreateAsset(peaksProfile, Path.Combine(assetPath, "New Peaks Profile.peaks.asset"));
            }
            else
            {
                string path = Path.GetDirectoryName(assetPath);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);

                ProjectWindowUtil.CreateAsset(peaksProfile, path + "/" + fileName + ".peaks.asset");
            }
        }

        [MenuItem("Assets/Create/SoundReactor/Peaks Profile", true)]
        static bool ValidatePeaksProfileCreate()
        {
            return (AssetDatabase.GetAssetPath(Selection.activeObject) != "");
        }


        [MenuItem("Tools/SoundReactor/Peaks Profile", false, 111)]
        static void PeaksProfileAsset()
        {
            PeaksProfileCreate();
        }

        [MenuItem("Tools/SoundReactor/Peaks Profile", true)]
        static bool ValidatePeaksProfileAsset()
        {
            return ValidatePeaksProfileCreate();
        }
        #endregion
        
        #region Menu Tools Component
        [MenuItem("Tools/SoundReactor/Component/SpectrumSource", false, 1)]
        static void SpectrumSourceToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(SpectrumSource));
        }

        [MenuItem("Tools/SoundReactor/Component/SpectrumSource", true)]
        static bool ValidateSpectrumSourceToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Component/SpectrumFilter", false, 1)]
        static void LevelSourceToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(SpectrumFilter));
        }

        [MenuItem("Tools/SoundReactor/Component/SpectrumFilter", true)]
        static bool ValidateLevelSourceToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }
        

        [MenuItem("Tools/SoundReactor/Component/Level", false, 1)]
        static void LevelToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(Level));
        }

        [MenuItem("Tools/SoundReactor/Component/Level", true)]
        static bool ValidateLevelToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Component/EQ", false, 1)]
        static void EQToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(EQ));
        }

        [MenuItem("Tools/SoundReactor/Component/EQ", true)]
        static bool ValidateEQToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Component/Builder", false, 50)]
        static void BuilderToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(SpectrumBuilder));
        }

        [MenuItem("Tools/SoundReactor/Component/Builder", true)]
        static bool ValidateBuilderToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Event", false, 1)]
        static void EventDriverToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(EventDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Event", true)]
        static bool ValidateEventDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Position", false, 1)]
        static void PositionDriverToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(PositionDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Position", true)]
        static bool ValidatePositionDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Rotation", false, 1)]
        static void RotationDriverToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(RotateDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Rotation", true)]
        static bool ValidateRotationDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Scale", false, 1)]
        static void ScaleDriverToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(ScaleDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Scale", true)]
        static bool ValidateScaleDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Color", false, 1)]
        static void ColorDriverToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(ColorDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Color", true)]
        static bool ValidateColorDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }


        [MenuItem("Tools/SoundReactor/Driver/MIDI Track Color", false, 1)]
        static void MidiTrackColorDriverToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(MidiTrackColorDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/MIDI Track Color", true)]
        static bool ValidateMidiTrackColorDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Force", false, 1)]
        static void ForceDriverToolsComponent()
        {
            if (Selection.activeGameObject)
            {
                Undo.AddComponent(Selection.activeGameObject, typeof(ForceDriver));
            }
        }

        [MenuItem("Tools/SoundReactor/Driver/Force", true)]
        static bool ValidateForceDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/ParticleEmitter", false, 1)]
        static void ParticleEmitterDriverToolsComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(ParticleEmitterDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/ParticleEmitter", true)]
        static bool ValidateParticleEmitterDriverToolsComponent()
        {
            return (Selection.activeGameObject != null);
        }
        #endregion

        #region Menu Tools GameObject
        [MenuItem("Tools/SoundReactor/SpectrumSource", false, 50)]
        static void SpectrumSourceToolsGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumSource", new System.Type[] { typeof(SpectrumSource) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/SpectrumFilter", false, 50)]
        static void LevelSourceToolsGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/SpectrumFilter (EQ)", false, 50)]
        static void EQToolsGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter), typeof(EQ) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/Level", false, 50)]
        static void LevelToolsGameObject()
        {
            GameObject gameObject = new GameObject("Level", new System.Type[] { typeof(Level) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/Builder", false, 100)]
        static void BuilderToolsGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumBuilder", new System.Type[] { typeof(SpectrumBuilder) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/AudioMidiSync", false, 101)]
        static void AudioMidiSyncToolsGameObject()
        {
            GameObject gameObject = new GameObject("AudioMidiSync", new System.Type[] { typeof(AudioMidiSync), typeof(AudioSource), typeof(MidiSource) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }
        #endregion

        #region Menu GameObject
        #if UNITY_EDITOR
        [MenuItem("GameObject/SoundReactor/SpectrumSource", false, 11)]
        static void SpectrumSourceGameObject(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumSource", new System.Type[] { typeof(SpectrumSource) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/SpectrumFilter", false, 11)]
        static void SpectrumFilterGameObject(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/SpectrumFilter (EQ)", false, 11)]
        static void EQGameObject(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter), typeof(EQ) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/Level", false, 11)]
        static void LevelGameObject(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("Level", new System.Type[] { typeof(Level) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/Builder", false, 201)]
        static void BuilderGameObject(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumBuilder", new System.Type[] { typeof(SpectrumBuilder) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/AudioMidiSync", false, 201)]
        static void AudioMidiSyncGameObject(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("AudioMidiSync", new System.Type[] { typeof(AudioMidiSync), typeof(AudioSource), typeof(MidiSource) });

            ParentAndUndo(gameObject, menuCommand);
        }
        #endif
        #endregion
    }
}
