using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Effects
{
    /// Use this to create SO wrappers around effects, implemented as classes
    public abstract class TextEffectSoWrapper<T> : BaseTextEffectSo where T : ITextEffect
    {
        [Tooltip("Override tag from original class")]
        [SerializeField] private string _tag;
        [SerializeField] private T _effect;

        public override string Tag => _tag;
        public override void ProcessEffect(TMP_Text text, int startIdx, int endIdx) => _effect.ProcessEffect(text, startIdx, endIdx);

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_tag)) _tag = _effect.Tag;;
        }
    }
}