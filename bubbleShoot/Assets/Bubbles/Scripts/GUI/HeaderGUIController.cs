using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class HeaderGUIController : MonoBehaviour
    {
        [Header("Stars")]
        [SerializeField]
        private Image star1;
        [SerializeField]
        private Image star2;
        [SerializeField]
        private Image star3;
        [SerializeField]
        private GameObject starPrefab;
        [SerializeField]
        [Tooltip("Simply change image from empty to full or animate")]
        private bool changeImage; // 



        [Space(8)]
        [Header("Sprites")]
        [SerializeField]
        private Sprite starEmpty;
        [SerializeField]
        private Sprite starFull;

        [Space(8)]
        [Header("Score strip")]
        [SerializeField]
        private Image ScoreStrip;
        [SerializeField]
        private Text ScoreCount;
        [SerializeField]
        private RectTransform yellowLight;


        [Space(8)]
        [Header("Targets")]
        [SerializeField]
        private GUIObjectTargetHelper targetFish;
        [SerializeField]
        private GUIObjectTargetHelper targetLopTopRow;
        [SerializeField]
        private GUIObjectTargetHelper targetRaiseAcnhor;
        [SerializeField]
        private GUIObjectTargetHelper targetClock;

        private GameObject fullStar_1;
        private GameObject fullStar_2;
        private GameObject fullStar_3;
        private int oldCount = 0;

        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }
        private SoundMasterController MSound { get { return SoundMasterController.Instance; } }

        public static HeaderGUIController Instance;

        #region regular
        void Awake()
        {
            if (Instance) Destroy(Instance.gameObject);
            Instance = this;
        }

        void Start()
        {
            if (GameBoard.gMode == GameMode.Edit)
            {
                gameObject.SetActive(false);
                return;
            }
            Refresh();

            //set delegates
            MPlayer.ChangeScoreEvent += RefreshScoreStrip;
            MPlayer.ChangeStarsEvent += RefreshStars;
            MPlayer.ChangeStarsEvent += GetStarsSound;
        }

        private void OnDestroy()
        {
            MPlayer.ChangeScoreEvent -= RefreshScoreStrip;
            MPlayer.ChangeStarsEvent -= RefreshStars;
            MPlayer.ChangeStarsEvent -= GetStarsSound;
            if (ScoreCount) SimpleTween.Cancel(ScoreCount.gameObject, false);
            if (ScoreStrip) SimpleTween.Cancel(ScoreStrip.gameObject, false);
        }

        #endregion regular

        public void Refresh()
        {
            RefreshTargets();
            RefreshStars(MPlayer.StarCount);
            RefreshScoreStrip(MPlayer.LevelScore, MPlayer.AverageScore);
        }

        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }
        }

        #region stars
        internal void RefreshStars(int count)
        {
            if (changeImage)
                ChangeStarsImage(count);
            else
                SetStars(count, null);
        }

        /// <summary>
        /// Change star sprites from empty to full
        /// </summary>
        /// <param name="starCount"></param>
        private void ChangeStarsImage(int starCount)
        {
            if (star1) star1.sprite = (starCount < 1) ? starEmpty : starFull;
            if (star2) star2.sprite = (starCount < 2) ? starEmpty : starFull;
            if (star3) star3.sprite = (starCount < 3) ? starEmpty : starFull;
        }

        /// <summary>
        /// Instantiate stars objects with any animation
        /// </summary>
        /// <param name="starCount"></param>
        /// <param name="completeCallBack"></param>
        private void SetStars(int starCount, Action completeCallBack)
        {
            if (fullStar_1 && starCount < 1) Destroy(fullStar_1);
            if (fullStar_2 && starCount < 2) Destroy(fullStar_2);
            if (fullStar_3 && starCount < 3) Destroy(fullStar_3);

            TweenSeq ts = new TweenSeq();

            if (!fullStar_1 && starCount > 0)
            {
                ts.Add((callBack) =>
                {
                    fullStar_1 =  InstantiateNewStar(starPrefab,  star1.transform);
                    AnimateNewStar(fullStar_1, callBack);
                });
              
            }

            if (!fullStar_2 && starCount > 1)
            {
                ts.Add((callBack) =>
                {
                    fullStar_2 = InstantiateNewStar(starPrefab,  star2.transform);
                    AnimateNewStar(fullStar_2, callBack);
                });
            }

            if (!fullStar_3 && starCount > 2)
            {
                ts.Add((callBack) =>
                {
                    fullStar_3 = InstantiateNewStar(starPrefab, star3.transform);
                    AnimateNewStar(fullStar_3, callBack);
                });
            }

            ts.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });

            ts.Start();
        }

        private GameObject InstantiateNewStar(GameObject prefab, Transform target)
        {
            GameObject  star = Instantiate(prefab, target.position, Quaternion.identity);
            star.transform.localScale = target.lossyScale;
            star.transform.parent = target;
            return star;
        }

        private void AnimateNewStar(GameObject star, Action completeCallBack)
        {
            SimpleTween.Value(star, 0, 1, 0.5f).SetOnUpdate((val) =>
            {
              if(star) star.transform.localScale = new Vector3(val, val, val);
            }).AddCompleteCallBack(() =>
            {
                completeCallBack?.Invoke();
            }).SetEase(EaseAnim.EaseOutBounce);
        }

        private void GetStarsSound(int stars)
        {
            MSound.SoundPlayGetStar(0, null);
        }

        internal void RefreshTargets()
        {
            if (GameBoard.Instance && GameBoard.Instance.WController != null)
            {
                int c = 0;
                int tc = 0;
                LevelType lType = GameBoard.Instance.WController.GameLevelType;
                GameBoard.Instance.WController.GetCurrTarget(out c, out tc);
                targetFish.gameObject.SetActive(lType == LevelType.FishLevel);
                targetLopTopRow.gameObject.SetActive(lType == LevelType.LoopTopRowLevel);
                targetRaiseAcnhor.gameObject.SetActive(lType == LevelType.AnchorLevel);
                targetClock.gameObject.SetActive(lType == LevelType.TimeLevel);

                switch (lType)
                {
                    case LevelType.LoopTopRowLevel:
                        targetLopTopRow.SetData(c, tc);
                        break;
                    case LevelType.TimeLevel:
                        //targetLopTopRow.SetData(c, tc);
                        break;
                    case LevelType.AnchorLevel:
                        targetRaiseAcnhor.SetData(c, tc);
                        break;
                    case LevelType.FishLevel:
                        targetFish.SetData(c, tc);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion stars

        #region score scorestrip  
        internal void RefreshScoreStrip(int score, int averageScore)
        {
            // refresh score text
            if (ScoreCount)
            {
                int newCount = score;
                SimpleTween.Cancel(ScoreCount.gameObject, false);
                SimpleTween.Value(ScoreCount.gameObject, oldCount, newCount, 0.5f).SetOnUpdate((float val) =>
                {
                    oldCount = (int)val;
                    ScoreCount.text = oldCount.ToString();
                });
            }

            //Refresh score strip
            if (!ScoreStrip) return;
            SimpleTween.Cancel(ScoreStrip.gameObject, false);
            float sizeX = ScoreStrip.GetComponent<RectTransform>().sizeDelta.x;
            float amount = (averageScore > 0) ? (float)score / (float)(averageScore) : 0;

            SimpleTween.Value(ScoreStrip.gameObject, ScoreStrip.fillAmount, amount, 0.3f).SetOnUpdate((float val) =>
            {
                ScoreStrip.fillAmount = val;
                if (yellowLight)
                {
                    yellowLight.anchoredPosition = new Vector2(sizeX * Mathf.Min(ScoreStrip.fillAmount, 0.93f), yellowLight.anchoredPosition.y);
                }
            }).SetEase(EaseAnim.EaseOutCubic);
        }

        internal void RefreshScore(int score)
        {
            if (ScoreCount)
            {
                Debug.Log("score " + score);
                ScoreCount.text = score.ToString();
            }
            //if (ScoreComplete && MBoard.WinContr != null) ScoreComplete.SetActive(MBoard.WinContr.HasScoreTarget && (score >= MBoard.WinContr.ScoreTarget));
        }
        #endregion score scorestrip

        public void LoadMap()
        {
            SceneLoader.Instance.LoadScene(0);
        }
       
    }
}