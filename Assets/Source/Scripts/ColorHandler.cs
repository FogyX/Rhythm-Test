using LDG.SoundReactor;
using UnityEngine;

namespace Source.Scripts
{
    public class ColorHandler : MonoBehaviour
    {
        [SerializeField] private Color _color;
    
        private SpriteRenderer _spriteRenderer;
    
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnLevel(PropertyDriver driver)
        {
            float level = driver.LevelScalar();
            Color newColor = _color * level;
            newColor.a = 1.0f;
            _spriteRenderer.color = newColor;
        }
    }
}
