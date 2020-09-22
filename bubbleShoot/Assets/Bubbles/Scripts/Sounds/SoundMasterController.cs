using UnityEngine;
using System;

namespace Mkey
{
    public class SoundMasterController : SoundMaster
    {
        public static new SoundMasterController Instance;

        [Space(8, order = 0)]
        [Header("AudioClips", order = 1)]
        public AudioClip shoot;
        [SerializeField]
        private AudioClip getStar;

        #region regular
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("Awake match sm");
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        #endregion rgular

        #region play sounds
        public void SoundPlayShoot(float playDelay, Action callBack)
        {
            PlayClip(playDelay, shoot, callBack);
        }

        public void SoundPlayGetStar(float playDelay, Action callBack)
        {
            PlayClip(playDelay, getStar, callBack);
        }
        #endregion play sounds
    }
}