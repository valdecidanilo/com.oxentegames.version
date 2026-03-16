using UnityEditor;
using UnityEngine;

namespace HyperVersion.Editor
{
    public class EnvironmentSelectorWindow : EditorWindow
    {
        public static string SelectedEnvironment { get; private set; } = null;
        public static bool Confirmed { get; private set; } = false;

        private static readonly string[] Options = { "Development", "Hml", "Release" };
        private int _selectedIndex = 0;

        public static void Show()
        {
            SelectedEnvironment = null;
            Confirmed = false;

            var window = CreateInstance<EnvironmentSelectorWindow>();
            window.titleContent = new GUIContent("Selecionar Ambiente");
            window.minSize = new Vector2(300, 130);
            window.maxSize = new Vector2(300, 130);
            window.ShowModalUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Escolha o ambiente para esta build:", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            _selectedIndex = EditorGUILayout.Popup("Ambiente:", _selectedIndex, Options);

            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Confirmar", GUILayout.Height(30)))
            {
                SelectedEnvironment = Options[_selectedIndex].ToLower();
                Confirmed = true;
                Close();
            }

            if (GUILayout.Button("Cancelar", GUILayout.Height(30)))
            {
                SelectedEnvironment = null;
                Confirmed = false;
                Close();
            }

            GUILayout.EndHorizontal();
        }
    }
}