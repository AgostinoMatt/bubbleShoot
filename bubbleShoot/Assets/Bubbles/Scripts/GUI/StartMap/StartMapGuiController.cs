using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class StartMapGuiController : MonoBehaviour
    {
        [SerializeField]
        private Text lifesText;
        [SerializeField]
        private Text coinsText;
        [SerializeField]
        private Image infiniteIcon;
        [SerializeField]
        private Text timerText;

        #region temp
        private float restHours = 0;
        private float restMinutes = 0;
        private float restSeconds = 0;
        #endregion temp

        private BubblesGuiController MGui { get { return BubblesGuiController.Instance; } }
        private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }

        public static StartMapGuiController Instance;

        #region regular
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            MPlayer.ChangeCoinsEvent += RefreshCoins;
            MPlayer.ChangeLifeEvent += RefreshLife;
            MPlayer.StartInfiniteLifeEvent += RefreshInfiniteLife;
            MPlayer.EndInfiniteLifeEvent += RefreshInfiniteLife;
            if(timerText) timerText.text = restMinutes.ToString("00") + ":" + restSeconds.ToString("00");
            Refresh();
        }

        void OnGUI()
        {
            RefreshTimerText();
        }

        private void OnDestroy()
        {
            MPlayer.ChangeCoinsEvent -= RefreshCoins;
            MPlayer.ChangeLifeEvent -= RefreshLife;
            MPlayer.StartInfiniteLifeEvent -= RefreshInfiniteLife;
            MPlayer.EndInfiniteLifeEvent -= RefreshInfiniteLife;
        }
        #endregion regular

        private void RefreshTimerText()
        {
            LifeIncTimer lifeIncTimer = LifeIncTimer.Instance;
            InfiniteLifeTimer infiniteLifeTimer = InfiniteLifeTimer.Instance;
            if (timerText)
            {
                if(infiniteLifeTimer && infiniteLifeTimer.IsWork)
                {
                    if (restHours != infiniteLifeTimer.RestHours || restMinutes != infiniteLifeTimer.RestMinutes || restSeconds != infiniteLifeTimer.RestSeconds)
                    {
                        restHours = infiniteLifeTimer.RestHours;
                        restMinutes = infiniteLifeTimer.RestMinutes;
                        restSeconds = infiniteLifeTimer.RestSeconds;
                        timerText.text = restHours.ToString("00") + ":" + restMinutes.ToString("00") + ":" + restSeconds.ToString("00");
                    }
                    if (lifesText && lifesText.gameObject.activeSelf) lifesText.gameObject.SetActive(false);
                    if (infiniteIcon && !infiniteIcon.gameObject.activeSelf) infiniteIcon.gameObject.SetActive(true);
                    return;
                }

                if (lifeIncTimer)
                {
                    if (restMinutes != lifeIncTimer.RestMinutes || restSeconds != lifeIncTimer.RestSeconds)
                    {
                        restMinutes = lifeIncTimer.RestMinutes;
                        restSeconds = lifeIncTimer.RestSeconds;
                        timerText.text = restMinutes.ToString("00") + ":" + restSeconds.ToString("00");
                    }
                    if (lifesText && !lifesText.gameObject.activeSelf) lifesText.gameObject.SetActive(true);
                    if (infiniteIcon && infiniteIcon.gameObject.activeSelf) infiniteIcon.gameObject.SetActive(false);
                }
            }
        }

        private void Refresh()
        {
            RefreshLife(MPlayer.Life);
            RefreshCoins(MPlayer.Coins);
            RefreshTimerText();
        }

        /// <summary>
        /// Set all interactable as activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        #region event handlers
        private void RefreshCoins(int coins)
        {
            if (coinsText) coinsText.text = coins.ToString();
        }

        private void RefreshLife(int life)
        {
            if (lifesText) lifesText.text = life.ToString();
        }

        private void RefreshInfiniteLife()
        {
            if (infiniteIcon) infiniteIcon.gameObject.SetActive(MPlayer.HasInfiniteLife());
            if (lifesText) lifesText.enabled = !MPlayer.HasInfiniteLife();
        }
        #endregion event handlers

        public void ShowRealCoinsShop_Click()
        {
            MGui?.ShowCoinsShop();
        }

        public void ShowRealLifeShop_Click()
        {
            MGui?.ShowLifeShop();
        }

        public void ShowSettings_Click()
        {
            MGui?.ShowSettings();
        }
    }
}