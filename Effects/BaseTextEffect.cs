using TMPro;

namespace Oneiromancer.TMP.Effects
{
    /// Base class for text effects that processes it character by character
    [System.Serializable]
    public abstract class BaseTextEffect : ITextEffect
    {
        public abstract string Tag { get; }

        public void ProcessEffect(TMP_Text text, int startIdx, int endIdx)
        {
            for (int i = startIdx; i <= endIdx; i++)
            {
                var charInfo = text.textInfo.characterInfo[i];
                if (charInfo.character == ' ') continue;
                ApplyToCharacter(text, charInfo);
            }
        }
        
        protected abstract void ApplyToCharacter(TMP_Text text, TMP_CharacterInfo charInfo);
    }
}