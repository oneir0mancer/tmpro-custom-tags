using System.Collections;
using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Typewriter
{
    /// Component that will render given text character-by-character, preserving custom tag effects.
    public class TypewriterAnimation : MonoBehaviour
    {
        public event System.Action TickEvent;
        public event System.Action<char> CharacterRenderedEvent;
        
        [SerializeField] private TMP_Text _text;
        [SerializeField, Min(0)] private float _delayBetweenCharacters = 0.1f;
        [Tooltip("Override time delay for a set of specific characters")]
        [SerializeField] private CharacterDelayOverride[] _delayOverrides;
        [SerializeField] private bool _playOnStart;

        private Coroutine _coroutine;
        private TMP_MeshInfo[] _cache;

        // Subscribe to TMP event of redrawing mesh
        private void OnEnable() => TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTMProChanged);
        private void OnDisable() => TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTMProChanged);

        private void Start()
        {
            _text.maxVisibleCharacters = 0;
            if (_playOnStart) Play();
        }
        
        private void OnValidate()
        {
            _text ??= GetComponent<TMP_Text>();
        }

        /// Play typewriter animation, starting with the first character
        public void Play()
        {
            _coroutine ??= StartCoroutine(TypewriterCoroutine());
        }

        /// Pause typewriter animation
        public void Pause()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = null;
        }
        
        /// Stop and reset typewriter animation
        public void Stop()
        {
            Pause();
            _text.maxVisibleCharacters = 0;
        }
        
        /// Resume typewriter animation from the last rendered character
        public void Resume()
        {
            _coroutine ??= StartCoroutine(TypewriterCoroutine(_text.maxVisibleCharacters));
        }

        // Pause animation, skip animaiton cursor to the end and resume animation
        public void SkipAnimation()
        {
            Pause();
            _text.maxVisibleCharacters = _text.text.Length;
            Resume();
        }

        // Restarts animation from begining
        public void RestartAnimation()
        {
            Stop();
            Play();
        }

        private IEnumerator TypewriterCoroutine(int startIdx = 0)
        {
            _text.ForceMeshUpdate();
            var count = _text.textInfo.characterCount;

            yield return new WaitForSeconds(_delayBetweenCharacters);
            
            for (int i = startIdx; i < count; i++)
            {
                Tick(i);
                
                var currentChar = _text.textInfo.characterInfo[i].character;
                CharacterRenderedEvent?.Invoke(currentChar);
                
                var time = _delayBetweenCharacters;
                foreach (var characterOverride in _delayOverrides)
                {
                    if (characterOverride.Contains(currentChar)) time = characterOverride.TimeDelay;
                    break;
                }
                
                yield return new WaitForSeconds(time);
            }
        }

        private void Tick(int index)
        {
            _cache = _text.textInfo.CopyMeshInfoVertexData();    //Cache vertex information before redrawing mesh
            
            _text.maxVisibleCharacters = index + 1;
            TickEvent?.Invoke();
        }

        private void RestoreCache()
        {
            if (_cache == null) return;
            
            for (int i = 0; i < _text.textInfo.meshInfo.Length; i++)
            {
                _text.textInfo.meshInfo[i].mesh.vertices = _cache[i].vertices;    //Restore vertices positions
                _text.textInfo.meshInfo[i].mesh.colors32 = _cache[i].colors32;    //Restore vertices colors
                _text.UpdateGeometry(_text.textInfo.meshInfo[i].mesh, i);
            }
        }
        
        private void OnTMProChanged(Object obj)
        {
            if (obj == _text) RestoreCache();
        }
    }
}
