using System.Collections;
using Oneiromancer.TMP.Tags;
using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Typewriter
{
    /// Component that will render given text character-by-character, preserving custom tag effects.
    public class TypewriterAnimation : MonoBehaviour
    {
        public event System.Action TickEvent;
        public event System.Action<char> CharacterRenderedEvent;
        public event System.Action AnimationStartedEvent;
        public event System.Action AnimationEndedEvent;
        
        [SerializeField] private TMP_Text _text;
        [SerializeField, Min(0)] private float _delayBetweenCharacters = 0.1f;
        [Tooltip("Override time delay for a set of specific characters")]
        [SerializeField] private CharacterDelayOverride[] _delayOverrides;
        [Tooltip("Marks that animated text uses custom tags")]
        [SerializeField] private bool _isUsedByTagParser;
        [SerializeField] private bool _playOnStart;

        private Coroutine _coroutine;
        private TMP_MeshInfo[] _cache;

        // Subscribe to TMP event of redrawing mesh
        private void OnEnable()
        {
            //TODO: for some reason subscribing without TagParser eats last symbol
            if (_isUsedByTagParser) TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTMProChanged);
        }

        private void OnDisable()
        {
            if (_isUsedByTagParser) TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTMProChanged);
        }

        private void Start()
        {
            _text.maxVisibleCharacters = 0;
            if (_playOnStart) Play();
        }
        
        private void OnValidate()
        {
            if (!_isUsedByTagParser) _isUsedByTagParser = TryGetComponent(out TagParser _);
            _text ??= GetComponent<TMP_Text>();
        }

        /// Play typewriter animation, starting with the first character
        [ContextMenu("Play")]
        public void Play()
        {
            _coroutine ??= StartCoroutine(TypewriterCoroutine());
        }

        /// Pause typewriter animation
        [ContextMenu("Pause")]
        public void Pause()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = null;
        }

        /// Resume typewriter animation from the last rendered character
        [ContextMenu("Resume")]
        public void Resume()
        {
            _coroutine ??= StartCoroutine(TypewriterCoroutine(_text.maxVisibleCharacters));
        }

        /// Stop and reset typewriter animation
        [ContextMenu("Reset")]
        public void StopAndReset()
        {
            Pause();
            _text.maxVisibleCharacters = 0;
        }

        /// Stop typewriter animation and reveal all the text
        [ContextMenu("Skip")]
        public void SkipAnimation()
        {
            Pause();
            _text.maxVisibleCharacters = int.MaxValue;
        }

        private IEnumerator TypewriterCoroutine(int startIdx = 0)
        {
            _text.ForceMeshUpdate();
            var count = _text.textInfo.characterCount;
            
            AnimationStartedEvent?.Invoke();

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
            
            _text.maxVisibleCharacters = int.MaxValue;
            AnimationEndedEvent?.Invoke();
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
