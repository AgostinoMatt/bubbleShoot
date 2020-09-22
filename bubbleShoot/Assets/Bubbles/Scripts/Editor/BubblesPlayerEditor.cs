using UnityEngine;
using UnityEditor;

namespace Mkey
{
    [CustomEditor(typeof(BubblesPlayer))]
    public class BubblesPlayerEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            #region test
            if (EditorApplication.isPlaying)
            {
                if (test = EditorGUILayout.Foldout(test, "Test"))
                {
                    BubblesPlayer t = (BubblesPlayer)target;

                    #region coins
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Add 500 coins"))
                    {
                        t?.AddCoins(500);
                    }

                    if (GUILayout.Button("Clear coins"))
                    {
                        t?.SetCoinsCount(0);
                    }
 
                    if (GUILayout.Button("Add coins -500"))
                    {
                        t.AddCoins(-500);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion coins

                    #region scenes
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Scene 0"))
                    {
                        SceneLoader.Instance?.LoadScene(0);
                    }
                    if (GUILayout.Button("Scene 1"))
                    {
                        SceneLoader.Instance?.LoadScene(1);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion scenes

                    #region stars
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Inc stars"))
                    {
                        t?.AddStars(1);
                    }

                    if (GUILayout.Button("Dec stars"))
                    {
                        t?.AddStars(-1);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion stars

                    #region life
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Inc life"))
                    {
                       t.AddLifes(1);
                    }

                    if (GUILayout.Button("Dec life"))
                    {
                        t.AddLifes(-1);
                    }

                    if (GUILayout.Button("Clean infinite life"))
                    {
                        t.CleanInfiniteLife();
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion life

                    #region score
                    EditorGUILayout.BeginHorizontal("box");

                    if (GUILayout.Button("Add score 200"))
                    {
                        t.AddScore(200);
                    }

                    EditorGUILayout.EndHorizontal();
                    #endregion score

                    if (GUILayout.Button("Reset to default"))
                    {
                        t.SetDefaultData();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Goto play mode for test");
            }
            #endregion test
        }
    }
}