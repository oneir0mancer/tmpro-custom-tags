using System.Linq;
using Oneiromancer.TMP.Effects;
using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Tags
{
    /// Component that can process custom tags in TMP_Text, given SO settings for each tag
    [ExecuteAlways]
    public class TagParser : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private BaseTextEffectSo[] _tagEffects;

        private CustomTagPreprocessor _currentPreprocessor;
        private bool _inPreviewMode;

        private void Awake()
        {
            SetParser();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !_inPreviewMode) return;
#endif
            UpdateTextMesh();
        }

        private void OnValidate()
        {
            _text ??= GetComponent<TMP_Text>();
        }

        public void SetTargetText(TMP_Text text)
        {
            if (text == null) throw new System.ArgumentNullException(nameof(text), "Text shouldn't be null");
            _text = text;
        }

        private void UpdateTextMesh()
        {
            if (_currentPreprocessor.TagInfos == null) return;
            _text.ForceMeshUpdate();
            foreach (var tagInfo in _currentPreprocessor.TagInfos)
            {
                foreach (var processor in _tagEffects)
                {
                    if (!tagInfo.IsTagEqual(processor.Tag)) continue;
                    processor.ProcessEffect(_text, tagInfo.StartIndex, tagInfo.LastIndex);
                }
            }
            _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        private void SetParser()
        {
            if (_tagEffects == null) return;
            var possibleTags = _tagEffects.Select(x => x.Tag).ToList();
            _currentPreprocessor = new CustomTagPreprocessor(possibleTags);
            _text.textPreprocessor = _currentPreprocessor;
            _text.ForceMeshUpdate();
        }

#if UNITY_EDITOR
        [ContextMenu("Start Preview")]
        public void Preview()
        {
            if (Application.isPlaying) return;
            SetParser();
            _inPreviewMode = true;
        }

        [ContextMenu("Stop Preview")]
        public void ResetParser()
        {
            if (Application.isPlaying) return;
            _currentPreprocessor = null;
            _text.textPreprocessor = null;
            _text.ForceMeshUpdate();
            _inPreviewMode = false;
        }
#endif
    }
}