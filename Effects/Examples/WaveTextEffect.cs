using TMPro;
using UnityEngine;

namespace Oneiromancer.TMP.Effects
{
    [System.Serializable]
    public class WaveTextEffect : BaseTextEffect
    {
        public override string Tag => "wave";
        
        [SerializeField] private float _speed;
        [SerializeField] private Vector2 _frequency;
        [SerializeField] private Vector2 _amplitude = Vector2.one;
        
        protected override void ApplyToCharacter(TMP_Text text, TMP_CharacterInfo charInfo)
        {
            int materialIndex = charInfo.materialReferenceIndex;
            Vector3[] newVertices = text.textInfo.meshInfo[materialIndex].vertices;

            for (int vertexIdx = charInfo.vertexIndex; vertexIdx < charInfo.vertexIndex + 4; vertexIdx++)
            {
                Vector3 offset = new Vector2(
                    _amplitude.x * Mathf.Sin(Time.realtimeSinceStartup * _speed + vertexIdx * _frequency.x),
                    _amplitude.y * Mathf.Cos(Time.realtimeSinceStartup * _speed + vertexIdx * _frequency.y));
                
                newVertices[vertexIdx] += offset;
            }
        }
    }
}