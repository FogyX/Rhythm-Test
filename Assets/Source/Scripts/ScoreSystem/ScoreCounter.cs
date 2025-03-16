using System;
using System.Globalization;
using VContainer;

namespace Source.Scripts
{
    public class ScoreCounter : IDisposable
    {
        private readonly Receptor _receptor;

        private readonly float _perfectGradeScore;
        private readonly float _greatGradeScore;
        private readonly float _goodGradeScore;
        
        private readonly int _comboToUpgradeMultiplier;
        private readonly float _comboMultiplierIncrement;
        
        private float _score;
        private int _combo;
        private float _comboMultiplier = 1f;

        public event Action<float> ScoreUpdated;
        public event Action<int> ComboUpdated;
        public event Action<NoteTouchGrade> GotGrade;

        [Inject]
        public ScoreCounter(Receptor receptor, GameConfig config)
        {
            _receptor = receptor;
            _perfectGradeScore = config.PerfectGradeScore;
            _greatGradeScore = config.GreatGradeScore;
            _goodGradeScore = config.GoodGradeScore;
            _comboToUpgradeMultiplier = config.ComboToUpgradeMultiplier;
            _comboMultiplierIncrement = config.ComboMultiplierIncrement;

            receptor.NoteTouched += NoteTouched;
        }

        private void NoteTouched(NoteTouchGrade grade)
        {
            switch (grade)
            {
                case NoteTouchGrade.Miss:
                    _combo = 0;
                    _comboMultiplier = 1f;
                    break;
                case NoteTouchGrade.Good:
                    _score += _goodGradeScore * _comboMultiplier;
                    break;
                case NoteTouchGrade.Great:
                    _score += _greatGradeScore * _comboMultiplier;
                    break;
                case NoteTouchGrade.Perfect:
                    _score += _perfectGradeScore * _comboMultiplier;
                    break;
            }

            if (grade != NoteTouchGrade.Miss)
            {
                _combo += 1;
                if (_combo % _comboToUpgradeMultiplier == 0)
                    _comboMultiplier += _comboMultiplierIncrement;
            }

            ScoreUpdated?.Invoke(_score);
            ComboUpdated?.Invoke(_combo);
            GotGrade?.Invoke(grade);
        }

        public void Dispose()
        {
            _receptor.NoteTouched -= NoteTouched;
        }
    }
}