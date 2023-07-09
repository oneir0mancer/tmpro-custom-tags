using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Effects
{
    /// Abstract class that is used to reference text effects in inspector as ScriptableObjects
    public abstract class BaseTextEffectSo: ScriptableObject, ITextEffect
    {
        public abstract string Tag { get; }
        public abstract void ProcessEffect(TMP_Text text, int startIdx, int endIdx);
    }
}