using UnityEditor;
using UnityEngine;

namespace Thew.KnobSock.Editor
{
    [InitializeOnLoad]
    public class KnobEditorClient
    {
        static KnobEditorClient()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        static void OnEditorUpdate()
        {
            if (Knobs.HasNewValues())
            {
                for (int i = 0; i < Knobs.NUM_KNOBS; i++)
                    Shader.SetGlobalFloat($"Knob{i}", Knobs.Get(i));
                
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    }
}