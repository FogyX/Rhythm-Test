using System.Globalization;
using TMPro;
using UnityEngine;

namespace Source.Scripts
{
    public class ScoreView : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _gradeText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _comboText;


        public void UpdateScoreText(float score)
        {
            _scoreText.text = score.ToString(CultureInfo.InvariantCulture);
        }

        public void UpdateComboText(int combo)
        {
            _comboText.text = $"x{combo}";
        }

        public void AnimateGradeText(NoteTouchGrade grade)
        {
            Color color = Color.clear;
            switch (grade)
            {
                case NoteTouchGrade.Miss:
                    color = Color.red;
                    break;
                case NoteTouchGrade.Good:
                    color = Color.yellow;
                    break;
                case NoteTouchGrade.Great:
                    color = Color.green;
                    break;
                case NoteTouchGrade.Perfect:
                    color = Color.cyan;
                    break;
            }
            
            _gradeText.color = color;
            _gradeText.text = grade.ToString().ToUpper();
            if (_gradeText.TryGetComponent(out Animator animator))
                animator.Play("ScaleTint");
        }   
    }
}