using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Effects
{
    [System.Serializable]
    public class RainbowTextEffect : BaseTextEffect
    {
        public override string Tag => "rainbow";
        
        [SerializeField] private float _speed;
        [SerializeField] private float _frequency;
        
        protected override void ApplyToCharacter(TMP_Text text, TMP_CharacterInfo charInfo)
        {
            int materialIndex = charInfo.materialReferenceIndex;
            Color32[] newColors = text.textInfo.meshInfo[materialIndex].colors32;

            for (int vertexIdx = charInfo.vertexIndex; vertexIdx < charInfo.vertexIndex + 4; vertexIdx++)
            {
                Color32 rainbow = Color.HSVToRGB(
                    (Time.realtimeSinceStartup * _speed + vertexIdx * (0.001f * _frequency)) % 1f,
                    1f, 1f);
                
                newColors[vertexIdx] = rainbow;
            }
        }
    }
}