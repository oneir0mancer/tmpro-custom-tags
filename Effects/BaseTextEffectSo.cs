using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Effects
{
    /// Use this SO class to reference text effects in inspector
    public abstract class BaseTextEffectSo: ScriptableObject, ITextEffect
    {
        public abstract string Tag { get; }
        public abstract void ProcessEffect(TMP_Text text, int startIdx, int endIdx);
    }
}