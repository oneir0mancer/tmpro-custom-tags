using TMPro;

namespace Oneiromancer.TMP.Effects
{
    public interface ITextEffect
    {
        string Tag { get; }
        void ProcessEffect(TMP_Text text, int startIdx, int endIdx);
    }
}