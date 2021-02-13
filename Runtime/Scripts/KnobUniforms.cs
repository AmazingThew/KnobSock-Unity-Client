using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thew.KnobSock
{
    [ExecuteAlways]
    public class KnobUniforms : MonoBehaviour
    {
        void Update()
        {
            for (int i = 0; i < Knobs.NUM_KNOBS; i++)
                Shader.SetGlobalFloat($"Knob{i}", Knobs.Get(i));
        }
    }
}