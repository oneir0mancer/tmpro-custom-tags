using UnityEngine;

namespace Oneiromancer.TMP.Typewriter
{
    [System.Serializable]
    public class CharacterDelayOverride
    {
        public float TimeDelay => _timeDelay;

        [SerializeField] private string _characters;
        [SerializeField, Min(0)] private float _timeDelay;

        public bool Contains(char character) => _characters.Contains(character);
    }
}