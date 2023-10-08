using System.Collections.Generic;
using System.Text;
using TMPro;

namespace Oneiromancer.TMP.Tags
{
    /// TMP Text Preprocessor, that is used to extract tag data from string, while also removing tags themselves for rendering.
    // TMP gives us an option to preprocess text before mesh is drawn. So we use it to remove tags from text,
    // while also storing where tags start and stop. The problem is that TMP processes it's tags after this,
    // so we need to keep all the unfamiliar tags in text, but skip them for tags' starts and stops.
    [System.Serializable]
    public class CustomTagPreprocessor : ITextPreprocessor
    {
        public IReadOnlyList<TagInfo> TagInfos => _tagInfos;

        private readonly List<TagInfo> _tagInfos = new List<TagInfo>();
        private readonly ICollection<string> _possibleTags;

        public CustomTagPreprocessor(ICollection<string> possibleTags)
        {
            _possibleTags = possibleTags;
        }

        /// Remove valid tags from a string, saving relevant tag info in process.
        public string PreprocessText(string text)
        {
            _tagInfos.Clear();
            StringBuilder builder = new StringBuilder();
            int builderIdx = 0;
            
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                {
                    if (ProcessTag(text, i, builderIdx, out int lastIdx))
                    {
                        i = lastIdx;    //skip to tag end
                    }
                    else if (SkipTmpTag(text, i, builder, out lastIdx))
                    {
                        i = lastIdx;    //basically add tag, but without increasing builderIdx
                    }
                }
                else
                {
                    builder.Append(text[i]);
                    builderIdx += 1;
                }
            }

            return builder.ToString();
        }

        private bool ProcessTag(string text, int rawTextIndex, int builderIdx, out int lastIndex)
        {
            if (!IsTag(text, rawTextIndex, out lastIndex)) return false;
            string tag = text.Substring(rawTextIndex + 1, lastIndex - rawTextIndex - 1);

            if (tag[0] != '/')
            {
                if (!_possibleTags.Contains(tag)) return false;
                _tagInfos.Add(new TagInfo(tag, builderIdx));
            }
            else
            {
                tag = tag.Substring(1);
                bool hit = false;
                foreach (var tagInfo in TagInfos)
                {
                    if (!tagInfo.IsTagEqualAndOpen(tag)) continue;
                    
                    tagInfo.Close(builderIdx - 1);    //Close first valid tag
                    hit = true;
                    break;
                }

                if (!hit) return false;    //If no tags were closed - don't remove
            }

            return true;
        }

        private bool SkipTmpTag(string text, int rawTextIndex, StringBuilder builder, out int lastIndex)
        {
            if (!IsTag(text, rawTextIndex, out lastIndex)) return false;
            string tag = text.Substring(rawTextIndex + 1, lastIndex - rawTextIndex - 1);
            if (tag[0] == '/') tag = tag.Substring(1);

            if (_possibleTags.Contains(tag)) return false;
            
            builder.Append(text.Substring(rawTextIndex, lastIndex - rawTextIndex + 1));
            return true;
        }

        private bool IsTag(string text, int rawTextIndex, out int lastIndex)
        {
            lastIndex = text.IndexOf('>', rawTextIndex);
            if (lastIndex == -1) return false;
            if (lastIndex - rawTextIndex - 1 <= 0) return false;   //no tag 

            return true;
        }
    }
}