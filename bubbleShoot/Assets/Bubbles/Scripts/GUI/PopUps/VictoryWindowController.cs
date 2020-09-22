using UnityEngine;
using UnityEngine.UI;
using System;
using Mkey;

public class VictoryWindowController : PopUpsController
{
    [SerializeField]
    private SceneCurve curveLeft;
    [SerializeField]
    private SceneCurve curveMiddle;
    [SerializeField]
    private SceneCurve curveRight;
    [SerializeField]
    private float speed = 5;

    [Space(8)]
    public Text scoreText;
    public Text missionDescriptionText;

    [Space(16)]
    public GameObject starLeftFull;
    public GameObject starMiddleFull;
    public GameObject starRightFull;
    [Space(8)]
    public GameObject starLeftEmpty;
    public GameObject starMiddleEmpty;
    public GameObject starRightEmpty;

    #region temp
    bool starLeftSet = false;
    bool starMiddleSet = false;
    bool starRightSet = false;
    TweenSeq ts;
    private GameBoard MBoard { get { return GameBoard.Instance; } }
    private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }
    #endregion temp

    #region regular
    private void Start()
    {
        MPlayer.ChangeScoreEvent += SetScore;
        MPlayer.ChangeStarsEvent += SetStars;
    }

    private void OnDestroy()
    {
        if (MPlayer) MPlayer.ChangeScoreEvent -= SetScore;
        if (MPlayer) MPlayer.ChangeStarsEvent -= SetStars;
        if (ts != null) ts.Break();

        SimpleTween.Cancel(gameObject, false);
        SimpleTween.Cancel(starRightFull, false);
        SimpleTween.Cancel(starLeftFull, false);
        SimpleTween.Cancel(starMiddleFull, false);
    }
    #endregion regular

    public override void RefreshWindow()
    {
        string description = (MPlayer.LcSet) ? MPlayer.LcSet.levelMission.Description : "";
        missionDescriptionText.text = description;
        missionDescriptionText.enabled = !string.IsNullOrEmpty(description);
        SetScore(MPlayer.LevelScore, MPlayer.AverageScore);
        SetStars(MPlayer.StarCount);
        base.RefreshWindow();
    }

    public void Replay_Click()
    {
        CloseWindow();
        SceneLoader.Instance.LoadScene(1);
    }

    public void Next_Click()
    {
        BubblesPlayer.CurrentLevel += 1;
        CloseWindow();
        SceneLoader.Instance.LoadScene(1);
    }

    private void SetStars(int count)
    {
        if (!starLeftSet) starLeftFull.SetActive(count >= 1);
        if (!starMiddleSet) starMiddleFull.SetActive(count >= 2);
        if (!starRightSet) starRightFull.SetActive(count >= 3);

        ts = new TweenSeq();
        if (count >= 1 && !starLeftSet)
        {
            starLeftSet = true;

            ts.Add((callBack) =>
            {
                if (curveLeft)
                {
                    float time = curveLeft.Length / speed;
                    curveLeft.MoveAlongPath(starLeftFull.gameObject, starLeftEmpty.transform, time, 0f, EaseAnim.EaseInOutSine, callBack);
                }
                else
                {
                    SimpleTween.Move(starLeftFull.gameObject, starLeftFull.transform.position, starLeftEmpty.transform.position, 0.5f).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                }

            });
        }
        if (count >= 2 && !starMiddleSet)
        {
            starMiddleSet = true;
            ts.Add((callBack) =>
            {
                if (curveMiddle)
                {
                    float time = curveMiddle.Length / speed;
                    curveMiddle.MoveAlongPath(starMiddleFull.gameObject, starMiddleEmpty.transform, time, 0f, EaseAnim.EaseInOutSine, callBack);
                }
                else
                {
                    SimpleTween.Move(starMiddleFull.gameObject, starMiddleFull.transform.position, starMiddleEmpty.transform.position, 0.5f).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                }
            });
        }
        if (count >= 3 && !starRightSet)
        {
            starRightSet = true;
            ts.Add((callBack) =>
            {
                if (curveRight)
                {
                    float time = curveRight.Length / speed;
                    curveRight.MoveAlongPath(starRightFull.gameObject, starRightEmpty.transform, time, 0f, EaseAnim.EaseInOutSine, callBack);
                }
                else
                {
                    SimpleTween.Move(starRightFull.gameObject, starRightFull.transform.position, starRightEmpty.transform.position, 0.5f).AddCompleteCallBack(() =>
                    {
                        callBack();
                    });
                }
            });
        }
        ts.Start();
    }

    private void SetScore(int score, int averageScore)
    {
        scoreText.text = score.ToString();
    }

    public void Cancel_Click()
    {
        CloseWindow();
        SceneLoader.Instance.LoadScene(0);
    }

    #region test
    public void TestLeftStar()
    {
        MPlayer.SetStarsCount(1);
    }

    public void TestMiddleStar()
    {
        MPlayer.SetStarsCount(2);
    }

    public void TestRightStar()
    {
        MPlayer.SetStarsCount(3);
    }
    #endregion test
}
