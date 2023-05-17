using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Core
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string MethodName { get; }
        public ButtonAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
