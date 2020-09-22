using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif
namespace Mkey
{
    public class BubblesGuiController : GuiController
    {
        [Space(8, order = 0)]
        [Header("Popup prefabs", order = 1)]
        public PopUpsController SettingsWindowPrefab;
        public PopUpsController VictoryWindowPrefab;
        public PopUpsController LossWindowPrefab;
        public PopUpsController AboutPrefab;
        public PopUpsController CoinsShopWindowPrefab;
        public PopUpsController LifeShopWindowPrefab;
        public PopUpsController BoosterMulticolorShopWindowPrefab;
        public PopUpsController BoosterEyeBallShopWindowPrefab;
        public PopUpsController BoosterFireBallShopWindowPrefab;
        public PopUpsController MissionPrefab;
        public WarningMessController TimeLeftPrefab;

        public Button modeButton;

        public static BubblesGuiController Instance;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); }
            else
            {
                Instance = this;
                Application.targetFrameRate = 35;
            }
        }

        protected override void Start()
        {
            base.Start();
#if UNITY_EDITOR
            if (modeButton)
            {
                modeButton.gameObject.SetActive(true);
                modeButton.GetComponentInChildren<Text>().text =(GameBoard.gMode == GameMode.Edit)? "GoTo" + System.Environment.NewLine + "PLAY": "GoTo" + System.Environment.NewLine + "EDIT";
                modeButton.onClick.AddListener(() =>
                {
                    if(GameBoard.gMode == GameMode.Edit)
                    {
                        GameBoard.gMode = GameMode.Play;
                        modeButton.GetComponentInChildren<Text>().text = "GoTo" + System.Environment.NewLine + "EDIT";
                    }
                    else
                    {
                        GameBoard.gMode = GameMode.Edit;
                        modeButton.GetComponentInChildren<Text>().text = "GoTo" + System.Environment.NewLine + "PLAY";
                    }
                    SceneLoader.Instance.ReLoadCurrentScene();
                });
            }
#else
            modeButton.gameObject.SetActive(false); 
#endif
        }

        #region menus
        public void ShowSettings()
        {
            ShowPopUp(SettingsWindowPrefab);
        }

        public void ShowVictory()
        {
            ShowPopUp(VictoryWindowPrefab);
        }

        public void ShowLoss()
        {
           ShowPopUp(LossWindowPrefab);
        }

        public void ShowMission(Action closeCallBack)
        {
            ShowPopUp(MissionPrefab);
        }

        public void ShowCoinsShop()
        {
            ShowPopUp(CoinsShopWindowPrefab);
        }

        public void ShowLifeShop()
        {
            ShowPopUp(LifeShopWindowPrefab);
        }

        public void ShowBoosterMulticolorShop()
        {
           ShowPopUp(BoosterMulticolorShopWindowPrefab);
        }

        public void ShowBoosterFireBallShop()
        {
            ShowPopUp(BoosterFireBallShopWindowPrefab);
        }

        public void ShowBoosterEyeBallShop()
        {
             ShowPopUp(BoosterEyeBallShopWindowPrefab);
        }

        internal void ShowAbout()
        {
            ShowPopUp(AboutPrefab);
        }
        #endregion menus

        #region messages
        internal void ShowMessageTimeLeft(string caption, string message, float showTime)
        {
            ShowMessage(TimeLeftPrefab, caption, message, showTime, null);
        }
        #endregion messages
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BubblesGuiController))]
    public class MatchGUIControllerEditor : Editor
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
                BubblesGuiController tg = (BubblesGuiController)target;
                if (test = EditorGUILayout.Foldout(test, "Test"))
                {
                    #region 
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Show time left"))
                    {
                        tg.ShowMessageTimeLeft("Warning", "5 moves left", 2);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion 
                }
                return;
            }
            EditorGUILayout.LabelField("Goto play mode for test");
            #endregion test
        }
    }
#endif
}


 
