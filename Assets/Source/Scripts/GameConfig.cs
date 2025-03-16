using Assets.Source.Scripts.LevelStructure;
using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Source.Scripts.InputSystem;

namespace Source.Scripts
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game Config", order = 0)]
    public class GameConfig : ScriptableObject
    {
        #region Grades

        [field: SerializeField] public float PerfectGradeScore { get; set; }
        [field: SerializeField] public float GreatGradeScore { get; set; }
        [field: SerializeField] public float GoodGradeScore { get; set; }

        #endregion

        #region CombosConfig

        [field: SerializeField] public int ComboToUpgradeMultiplier { get; set; }
        [field: SerializeField] public float ComboMultiplierIncrement { get; set; }

        #endregion

        #region LevelGeneration

        [field: SerializeField] public float SyncOffset { get; set; }
        [field: SerializeField] public int ChannelToGenerate { get; set; }
        [field: SerializeField] public float NoteMovementSpeed { get; set; }
        [field: SerializeField] public GameNote GameNotePrefab { get; set; }
        [field:SerializeField] public float NotesSpacing { get; set; }
        [field: SerializeField] public NotesContainer NotesContainerPrefab { get; set; }
        [field: SerializeField] public NoteButton NoteButtonPrefab { get; set; }
        #endregion

        #region GradesDistances

        [field: SerializeField] public float GoodGradeDistance { get; set; }
        [field: SerializeField] public float GreatGradeDistance { get; set; }
        [field: SerializeField] public float PerfectGradeDistance { get; set; }

        private void OnValidate()
        {
            if (PerfectGradeDistance < 0) PerfectGradeDistance = 0;
            if (GreatGradeDistance < 0) GreatGradeDistance = 0;
            if (GoodGradeDistance < 0) GoodGradeDistance = 0;
            
            if (PerfectGradeDistance > GreatGradeDistance) PerfectGradeDistance = GreatGradeDistance;
            if (GreatGradeDistance > GoodGradeDistance) GreatGradeDistance = GoodGradeDistance;
            if (PerfectGradeDistance > GoodGradeDistance) PerfectGradeDistance = GoodGradeDistance;
        }

        #endregion

        #region Input
        [field: SerializeField] public List<NoteInputPair> NoteInputPairs;
        #endregion

    }
        [Serializable]
        public struct NoteInputPair
        {
            [field: SerializeField] public int NoteId { get; set; }
            [field: SerializeField] public KeyCode Key { get; set; }
        }
}