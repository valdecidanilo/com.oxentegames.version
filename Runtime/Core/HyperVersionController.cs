using System.Runtime.InteropServices;
using UnityEngine;

namespace HyperVersion.Core
{
    public static class HyperVersionController
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void HyperVersionShow();

        [DllImport("__Internal")]
        private static extern void HyperVersionHide();
#endif

        public static void Show()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            HyperVersionShow();
#endif
        }

        public static void Hide()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            HyperVersionHide();
#endif
        }
    }
}