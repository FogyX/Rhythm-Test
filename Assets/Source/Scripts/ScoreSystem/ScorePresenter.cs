using System;
using VContainer;

namespace Source.Scripts
{
    public class ScorePresenter : IDisposable
    {
        private readonly ScoreView _scoreView;
        private readonly ScoreCounter _scoreCounter;

        [Inject]
        public ScorePresenter(ScoreView scoreView, ScoreCounter scoreCounter)
        {
            _scoreView = scoreView;
            _scoreCounter = scoreCounter;
            _scoreCounter.ScoreUpdated += OnScoreUpdated;
            _scoreCounter.ComboUpdated += OnComboUpdated;
            _scoreCounter.GotGrade += OnGotGrade;
        }

        private void OnScoreUpdated(float score)
        {
            _scoreView.UpdateScoreText(score);
        }

        private void OnComboUpdated(int combo)
        {
            _scoreView.UpdateComboText(combo);
        }

        private void OnGotGrade(NoteTouchGrade grade)
        {
            _scoreView.AnimateGradeText(grade);
        }

        public void Dispose()
        {
            _scoreCounter.ScoreUpdated -= OnScoreUpdated;
            _scoreCounter.ComboUpdated -= OnComboUpdated;
            _scoreCounter.GotGrade -= OnGotGrade;
            _scoreCounter.Dispose();
        }
    }
}