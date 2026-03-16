using UnityEditor;
using UnityEngine;

namespace HyperVersion.Editor
{
    public class BuildTagSelectorWindow : EditorWindow
    {
        public static string SelectedTag { get; private set; } = null;
        public static bool Confirmed { get; private set; } = false;

        private static readonly string[] Options = { "Nenhuma (Release)", "DEV", "HML" };
        private int _selectedIndex = 0;

        public static void Show()
        {
            SelectedTag = null;
            Confirmed = false;

            var window = CreateInstance<BuildTagSelectorWindow>();
            window.titleContent = new GUIContent("Tag de Versão");
            window.minSize = new Vector2(340, 150);
            window.maxSize = new Vector2(340, 150);

            var main = EditorGUIUtility.GetMainWindowPosition();
            var x = main.x + (main.width - 340) * 0.5f;
            var y = main.y + (main.height - 150) * 0.5f;
            window.position = new Rect(x, y, 340, 150);

            window.ShowModalUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Tag visual desta build:", EditorStyles.boldLabel);
            
            var helpStyle = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
            EditorGUILayout.LabelField("Apenas identifica a versão visualmente. Não afeta o ambiente técnico.", helpStyle);
            EditorGUILayout.Space(8);

            _selectedIndex = EditorGUILayout.Popup("Tag:", _selectedIndex, Options);

            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Confirmar", GUILayout.Height(30)))
            {
                SelectedTag = _selectedIndex switch
                {
                    0 => "",       // Release — sem tag
                    1 => "dev",
                    2 => "hml",
                    _ => ""
                };
                Confirmed = true;
                Close();
            }

            if (GUILayout.Button("Cancelar", GUILayout.Height(30)))
            {
                SelectedTag = null;
                Confirmed = false;
                Close();
            }

            GUILayout.EndHorizontal();
        }
    }
}