using UnityEditor;
using UnityEngine;

namespace Oneiromancer.TMP.Tags
{
    [CustomEditor(typeof(TagParser))]
    public class TagParserEditor : Editor
    {
        private bool _inPreview;
        
        public override void OnInspectorGUI()
        {
            var parser = target as TagParser;
            base.OnInspectorGUI();
            
            if (!_inPreview)
            {
                if (GUILayout.Button("Preview"))
                {
                    parser.Preview();
                    _inPreview = true;
                }
            }
            else
            {
                if (GUILayout.Button("Stop preview"))
                {
                    parser.ResetParser();
                    _inPreview = false;
                }
            }
        }

        private void OnDisable()
        {
            if (!_inPreview) return;
            var parser = target as TagParser;
            parser.ResetParser();
            _inPreview = false;
        }
    }
}