using UnityEngine;

namespace Oneiromancer.TMP.Tags
{
    /// Custom tag info: it's name, first and last affected text index.
    [System.Serializable]
    public class TagInfo
    {
        public string Tag => _tag;
        public int StartIndex => _startIndex;
        public int LastIndex => _lastIndex;

        [SerializeField] private string _tag;
        [SerializeField] private int _startIndex;
        [SerializeField] private int _lastIndex = -1;

        private int _tagHash;

        public TagInfo(string tag, int startIndex)
        {
            _tag = tag;
            _startIndex = startIndex;

            _tagHash = tag.GetHashCode();
        }

        public void Close(int lastIndex)
        {
            _lastIndex = lastIndex;
        }

        public bool IsTagEqualAndOpen(string tag)
        {
            return LastIndex == -1 && IsTagEqual(tag);
        }
        
        public bool IsTagEqual(string tag)
        {
            return tag.GetHashCode() == _tagHash;
        }
    }
}