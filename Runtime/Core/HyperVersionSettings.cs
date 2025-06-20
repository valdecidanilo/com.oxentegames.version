using UnityEngine;

namespace HyperVersion.Core
{
    [CreateAssetMenu(fileName = "HyperVersionSettings",
        menuName = "HyperVersion/Settings",
        order = 0)]
    public class HyperVersionSettings : ScriptableObject
    {
        public bool showDate   = true;
        public bool showBuild  = true;
        public bool showEnvTag = true;
    }
}
