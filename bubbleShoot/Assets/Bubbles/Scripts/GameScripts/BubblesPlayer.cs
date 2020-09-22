using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Mkey
{
    public enum HardMode { Easy, Hard }
    public class BubblesPlayer : MonoBehaviour
    {
        #region sets
        public GameConstructSet gcSet;

        public GameObjectsSet GoSet
        {
            get { return gcSet.GOSet; }
        }

        public LevelConstructSet LcSet
        {
            get { return gcSet.GetLevelConstructSet(CurrentLevel); }
        }
        #endregion sets

        #region default data
        [Space(10, order = 0)]
        [Header("Default data", order = 1)]
        [Tooltip("Default coins at start")]
        [SerializeField]
        private int defCoinsCount = 500;

        [Tooltip("Default lifes count, at start")]
        [SerializeField]
        private int defLifesCount = 5; 

        [Tooltip("Max stars count")]
        [SerializeField]
        private int maxStarsCount = 3;

        [Tooltip("Convert stars to coins")]
        [SerializeField]
        private bool starsToCoins = true;

        [Tooltip("Default facebook coins gift")]
        [SerializeField]
        public int defFBCoinsGift = 100;

        [Tooltip("Max lifes count")]
        [SerializeField]
        private int maxLifesCount = 5;

        [HideInInspector]
        [Tooltip("Default player name")]
        [SerializeField]
        private string defFullName = "Good Player";
        [Tooltip("Check if you want to save coins, level, progress, facebook gift flag, sound settings")]
        [SerializeField]
        private bool saveData = false;

        [HideInInspector]
        [Tooltip("Default Hard mode")]
        [SerializeField]
        private HardMode defHardMode = HardMode.Easy;
        #endregion default data

        #region keys
        private static string savePrefix = "mkbubbles_";
        private string saveCoinsKey = savePrefix + "coins"; // current coins
        private string saveLifeKey = savePrefix + "life"; // current lifes
        private string saveTopPassedLevelKey = savePrefix + "toppassedlevel";
        private string saveFbCoinsKey = savePrefix + "coinsfb"; // saved flag for facebook coins (only once)
        private string saveInfLifeTimeStampEnd = savePrefix + "inflifetsend"; // saved time stamp for infinite life end

        private string SaveLevelStarsKey { get { return CurrentLevel.ToString() + savePrefix + "levelstars"; } }  // level stars
        private string SaveLevelScoresKey { get { return CurrentLevel.ToString() + savePrefix + "levelscores"; } } // level score

        private string saveHardModeKey = savePrefix + "hardmode";
        private string saveFullName = savePrefix + "fullname";
        #endregion keys

        #region temporary stores
        private List<int> levelScores;  // temporary array for store levels scores
        private List<int> levelsStars;  // temporary array for store levels stars

        public IList<int> LevelsScoresStore { get { return levelScores.AsReadOnly(); } }
        public IList<int> LevelsStarsStore { get { return levelsStars.AsReadOnly(); } }
        #endregion temporary stores

        #region events
        public Action<int> ChangeCoinsEvent;
        public Action<int> ChangeLifeEvent;
        public Action<int> ChangeStarsEvent;
        public Action<int, int> ChangeScoreEvent;
        public Action PassLevelEvent;
        public Action StartInfiniteLifeEvent;
        public Action EndInfiniteLifeEvent;
        public Action<HardMode> ChangeHardMode;
        public Action<string> ChangeFullNameEvent;
        #endregion events

        #region properties
        /// <summary>
        ///  set from map,  value start from 0 for first game level
        /// </summary>
        public static int CurrentLevel
        {
            get; set;
        }

        public bool SaveData { get { return saveData; } }

        public BoostersHolder BoostHolder { get; private set; }
        #endregion properties

        #region per game saved properties
        public int TopPassedLevel { get; private set; } // set from PassLevel method,value start from 0 for first game level

        public int Coins { get; private set; }

        public int Life { get; private set; }

        public HardMode Hardmode { get; private set; }

        public string FullName { get; private set; }
        #endregion per game properties

        #region per level properties
        public int StarCount // per level, zero at level start
        {
            get; private set;

        }

        public int LevelScore // per level, zero at level start
        {
            get; private set;
        }

        public int AverageScore { get; private set; }
        #endregion per level properties

        public static BubblesPlayer Instance;

        #region regular
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            #region game sets 
            if (!gcSet)
            {
                Debug.Log("Game construct set not found!!!");
                return;
            }

            if (!LcSet)
            {
                Debug.Log("Level construct set not found!!! - " + CurrentLevel);
                return;
            }

            if (!GoSet)
            {
                Debug.Log("GoSet not found!!!");
                return;
            }
            #endregion game sets 

            levelsStars = new List<int>();
            levelScores = new List<int>();

            LoadCoins();
            LoadLife();
            LoadPassedLevel();
            LoadFullName();

            // load saved data
            if (SaveData)
            {
                levelScores = GetPassedLevelsScores();
                levelsStars = GetPassedLevelsStars();
            }
            Debug.Log("CurrentLevel: " + CurrentLevel);
            SetScore(0);
            BoostHolder = new BoostersHolder(GoSet, SaveData, savePrefix);
        }

        private void OnValidate()
        {

        }
        #endregion regular

        #region score
        public void AddScore(int scores)
        {
            SetScore(LevelScore + scores);
        }

        public void SetScore(int scores)
        {
            scores = Mathf.Max(0, scores);
            bool changed = (scores != LevelScore);
            LevelScore = scores;
            if (changed)
            {
                ChangeScoreEvent?.Invoke(LevelScore, AverageScore);
                SetStarsByScore();
            }
        }

        public void SetAverageScore(int averageScore)
        {
            AverageScore = Mathf.Max(1, averageScore); // no zero
        }

        /// <summary>
        /// Get score for level. If level not passed return 0;
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int GetLevelScore(int level)
        {
            int result = 0;
            string key = SaveLevelScoresKey;
            if (PlayerPrefs.HasKey(key)) result = PlayerPrefs.GetInt(key);
            return result;
        }

        /// <summary>
        /// Return list of scores for all passed levels
        /// </summary>
        /// <returns></returns>
        private List<int> GetPassedLevelsScores()
        {
            List<int> result = new List<int>(TopPassedLevel + 1);

            for (int i = 0; i < TopPassedLevel; i++)
            {
                result.Add(GetLevelScore(i));
            }
            return result;
        }

        private void StoreScoresTemporary()
        {
            int count = levelScores.Count;
            if (count <= CurrentLevel)
            {
                levelScores.AddRange(new int[CurrentLevel - count + 10]); // add 10 for next scores
            }
            if (LevelScore > levelScores[CurrentLevel]) levelScores[CurrentLevel] = LevelScore;
        }
        #endregion score

        #region stars
        public void AddStars(int count)
        {
            SetStarsCount(StarCount + count);
        }

        public void SetStarsCount(int count)
        {
            count = Mathf.Clamp(count, 0, maxStarsCount);
            bool changed = (StarCount != count);
            StarCount = count;
            if (changed) ChangeStarsEvent?.Invoke(StarCount); // Debug.Log("StarCount: " + StarCount);
        }

        /// <summary>
        /// Get stars for level. If level not passed return 0;
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int GetLevelStars(int level)
        {
            int result = 0;
            string key = level.ToString() + savePrefix + "levelstars";
            if (PlayerPrefs.HasKey(key)) result = PlayerPrefs.GetInt(key);
            return result;
        }

        /// <summary>
        /// Return list of scores for all passed levels
        /// </summary>
        /// <returns></returns>
        private List<int> GetPassedLevelsStars()
        {
            List<int> result = new List<int>(TopPassedLevel + 1);

            for (int i = 0; i <= TopPassedLevel; i++)
            {
                result.Add(GetLevelStars(i));
            }
            return result;
        }

        /// <summary>
        /// Store levels stars in temporary array
        /// </summary>
        private void StoreStarsTemporary()
        {
            int count = levelsStars.Count;
            if (count <= CurrentLevel)
            {
                levelsStars.AddRange(new int[CurrentLevel - count + 10]); // add 10 for next levels stars
            }
            if (StarCount > levelsStars[CurrentLevel]) levelsStars[CurrentLevel] = StarCount;
        }

        private void SetStarsByScore()
        {
            int stars = 0;
            float starPos_1 = 0.45f;
            float starPos_2 = 0.75f; 
            float starPos_3 = 1f;
            int maxScore = AverageScore;

            if (LevelScore > (maxScore * starPos_3)) stars = 3;
            else if (LevelScore > (maxScore * starPos_2)) stars = 2;
            else if (LevelScore > (maxScore * starPos_1)) stars = 1;
            SetStarsCount(stars);
        }
        #endregion stars

        #region life
        /// <summary>
        /// Add lifes count and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddLifes(int count)
        {
             SetLifesCount(Life + count);
        }

        /// <summary>
        /// Set lifes count and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetLifesCount(int count)
        {
            if (HasInfiniteLife())
            {
                Life = maxLifesCount;
                return;
            }
            count = Mathf.Clamp(count, 0, maxLifesCount);
            bool changed = (Life != count);
            Life = count;
            if (SaveData && changed)
            {
                string key = saveLifeKey;
                PlayerPrefs.SetInt(key, Life);
            }
            if (changed) ChangeLifeEvent?.Invoke(Life);
        }

        /// <summary>
        /// Load serialized life or set defaults
        /// </summary>
        private void LoadLife()
        {
            if (SaveData)
            {
                string key = saveLifeKey;
                if (PlayerPrefs.HasKey(key)) SetLifesCount(PlayerPrefs.GetInt(key));
                else SetLifesCount(defLifesCount);
            }
            else
            {
                SetLifesCount(defLifesCount);
            }
        }

        public void SetDefaultLife()
        {
            SetLifesCount(defLifesCount);
        }

        public void StartInfiniteLife(int hours)
        {
            SetLifesCount(maxLifesCount);
            PlayerPrefs.SetString(saveInfLifeTimeStampEnd, DateTime.Now.AddHours(hours).ToString(CultureInfo.InvariantCulture));
            Debug.Log("End inf life: " + (DateTime.Now + new TimeSpan(hours, 0, 0)));
            StartInfiniteLifeEvent?.Invoke();
        }

        public void CleanInfiniteLife()
        {
            PlayerPrefs.SetString(saveInfLifeTimeStampEnd, DateTime.Now.ToString(CultureInfo.InvariantCulture));
            LoadLife();
            EndInfiniteLifeEvent?.Invoke();
        }

        public void EndInfiniteLife()
        {
            LoadLife();
            EndInfiniteLifeEvent?.Invoke();
        }

        public bool HasInfiniteLife()
        {
            if (!PlayerPrefs.HasKey(saveInfLifeTimeStampEnd)) return false;
            DateTime end = GlobalTimer.DTFromSring(PlayerPrefs.GetString(saveInfLifeTimeStampEnd));// Debug.Log("end: "+end);
            return (DateTime.Now < end);
        }

        public TimeSpan GetInfLifeTimeRest()
        {
            if (!PlayerPrefs.HasKey(saveInfLifeTimeStampEnd)) return new TimeSpan(0, 0, 0);
            DateTime end = GlobalTimer.DTFromSring(PlayerPrefs.GetString(saveInfLifeTimeStampEnd));
            return end - DateTime.Now;
        }
        #endregion life

        #region coins
        /// <summary>
        /// Add coins and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddCoins(int count)
        {
            SetCoinsCount(Coins + count);
        }

        /// <summary>
        /// Set coins and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetCoinsCount(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (Coins != count);
            Coins = count;
            if (SaveData && changed)
            {
                string key = saveCoinsKey;
                PlayerPrefs.SetInt(key, Coins);
            }
            if (changed) ChangeCoinsEvent?.Invoke(Coins);
        }

        /// <summary>
        /// Add facebook gift (only once), and save flag.
        /// </summary>
        public void AddFbCoins()
        {
            if (!PlayerPrefs.HasKey(saveFbCoinsKey) || PlayerPrefs.GetInt(saveFbCoinsKey) == 0)
            {
                PlayerPrefs.SetInt(saveFbCoinsKey, 1);
                AddCoins(defFBCoinsGift);
            }
        }

        /// <summary>
        /// Load serialized coins or set defaults
        /// </summary>
        private void LoadCoins()
        {
            if (SaveData)
            {
                string key = saveCoinsKey;
                if (PlayerPrefs.HasKey(key)) SetCoinsCount(PlayerPrefs.GetInt(key));
                else SetCoinsCount(defCoinsCount);
            }
            else
            {
                SetCoinsCount(defCoinsCount);
            }
        }

        public void SetDefaultCoins()
        {
            SetCoinsCount(defCoinsCount);
        }
        #endregion coins

        #region level
        public void PassLevel()
        {
            if (CurrentLevel > TopPassedLevel)
                TopPassedLevel = CurrentLevel; 

            if(starsToCoins)AddCoins(StarCount);

            // store temporary data
            StoreScoresTemporary();
            StoreStarsTemporary();

            //save per level data
            if (SaveData)
            {
                // save stars
                int oldStarCount = 0;
                string key = SaveLevelStarsKey;
                if (PlayerPrefs.HasKey(key))
                    oldStarCount = PlayerPrefs.GetInt(key);
                if (oldStarCount < StarCount)
                    PlayerPrefs.SetInt(key, StarCount); // save only best stars result

                // save score
                int oldScore = 0;
                key = SaveLevelScoresKey;
                if (PlayerPrefs.HasKey(key))
                    oldScore = PlayerPrefs.GetInt(key);
                if (oldScore < LevelScore)
                    PlayerPrefs.SetInt(key, LevelScore); // save only best score result

                // save top passed level
                key = saveTopPassedLevelKey;
                PlayerPrefs.SetInt(key, TopPassedLevel);

            }
            PassLevelEvent?.Invoke();
        }

        private void LoadPassedLevel()
        {
            string key = saveTopPassedLevelKey;
            if (SaveData)
            {
                if (PlayerPrefs.HasKey(key))
                    TopPassedLevel = PlayerPrefs.GetInt(key);
                else TopPassedLevel = -1;
            }
            else TopPassedLevel = -1;
        }
        #endregion level

        #region hard mode
        /// <summary>
        /// Set hard mode
        /// </summary>
        /// <param name="count"></param>
        public void SetHardMode(HardMode hMode)
        {
            bool changed = (Hardmode != hMode);
            Hardmode = hMode;
            if (SaveData && changed)
            {
                string key = saveHardModeKey;
                PlayerPrefs.SetInt(key, (int)Hardmode);
            }
            if (changed) ChangeHardMode?.Invoke(Hardmode);
            Debug.Log("mode :" + hMode);
        }

        private void LoadHardMode()
        {
            if (SaveData)
            {
                string key = saveHardModeKey;
                if (PlayerPrefs.HasKey(key)) SetHardMode((HardMode)PlayerPrefs.GetInt(key));
                else SetHardMode(defHardMode);
            }
            else
            {
                SetHardMode(defHardMode);
            }
        }

        public void SetDefaultHardMode()
        {
            SetHardMode(defHardMode);
        }
        #endregion hard mode

        #region full name
        /// <summary>
        /// Set full name
        /// </summary>
        /// <param name="count"></param>
        public void SetFullName(string fName)
        {
            fName = string.IsNullOrEmpty(fName) ? FullName : fName;
            bool changed = (FullName != fName);
            FullName = fName;
            if (SaveData && changed)
            {
                string key = saveFullName;
                PlayerPrefs.SetString(key, FullName);
            }
            if (changed) ChangeFullNameEvent?.Invoke(FullName);
            Debug.Log("Full Name :" + FullName);
        }

        private void LoadFullName()
        {
            if (SaveData)
            {
                string key = saveFullName;
                SetFullName(PlayerPrefs.GetString(key, defFullName));
            }
            else
            {
                SetFullName(defFullName);
            }
        }

        public void SetDefaultFullName()
        {
            SetFullName(defFullName);
        }
        #endregion full name

        #region boosters
        /// <summary>
        /// Create boosterholder, if not created
        /// </summary>
        /// <param name="goSet"></param>
        public void CreateBoosterHolder(GameObjectsSet goSet)
        {
            if (BoostHolder == null)
                BoostHolder = new BoostersHolder(goSet, SaveData, savePrefix);
        }
        #endregion boosters

        public void SetDefaultData()
        {
            SetDefaultCoins();
            PlayerPrefs.SetInt(saveFbCoinsKey, 0); // reset facebook gift
            SetDefaultLife();
            SetDefaultFullName();
            EndInfiniteLife();
            TopPassedLevel = -1;
        }
    }
}