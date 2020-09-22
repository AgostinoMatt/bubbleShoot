using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Mkey
{
    [CreateAssetMenu]
    public class LevelConstructSet : BaseScriptable
    {
        [SerializeField]
        private int vertSize = 15;
        [SerializeField]
        private int horSize = 11;
        [HideInInspector]
        [SerializeField]
        public float distX = 0.15f;
        [HideInInspector]
        [SerializeField]
        public float distY = 0.15f;
        [HideInInspector]
        [SerializeField]
        public float scale = 1.0f;
        [SerializeField]
        public int backGroundNumber = 0;

        public int BackGround
        {
            get { return backGroundNumber; }

        }

        public MissionConstruct levelMission;

        public int VertSize
        {
            get { return vertSize; }
            set
            {
                if (value < 1) value = 1;
                vertSize = value;
                SetAsDirty();
            }
        }

        public int HorSize
        {
            get { return horSize; }
            set
            {
                if (value < 1) value = 1;
                horSize = value;
                SetAsDirty();
            }
        }

        public float DistX
        {
            get { return distX; }
            set
            {
                distX = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float DistY
        {
            get { return distY; }
            set
            {
                distY = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                if (value < 0) value = 0;
                scale = RoundToFloat(value, 0.05f);
                SetAsDirty();
            }
        }

        [SerializeField]
        public List<CellData> featuredCells;
        [SerializeField]
        public List<CellData> overlays;

        #region regular
        void OnEnable()
        {
           // Debug.Log("onenable " + ToString());
            if (levelMission == null) levelMission = new MissionConstruct();
            levelMission.SaveEvent = SetAsDirty;

        }

        void Awake()
        {
           // Debug.Log("awake " + ToString());
        }
        #endregion regular

        public void AddFeaturedCell(CellData cd)
        {
            if (featuredCells == null) featuredCells = new List<CellData>(1);
            RemoveCellData(featuredCells, cd);
            featuredCells.Add(cd);
            SetAsDirty();
        }

        public void RemoveFeaturedCell(CellData cd)
        {
            if (featuredCells == null) featuredCells = new List<CellData>();
            RemoveCellData(featuredCells, cd);
            RemoveCellData(overlays, cd);
            SetAsDirty();
        }

        public void AddOverlay(CellData cd)
        {
            if (overlays == null) overlays = new List<CellData>(1);
            RemoveCellData(overlays, cd);
            overlays.Add(cd);
            SetAsDirty();
        }

        public void RemoveOverlayCell(CellData cd)
        {
            if (overlays == null) overlays = new List<CellData>();
            RemoveCellData(overlays, cd);
            SetAsDirty();
        }

        public void Clean(GameObjectsSet gOS)
        {
            Action<List<CellData>> cAction = (arr) =>
            {
                if (arr != null)
                {
                    arr.RemoveAll((c) =>
                    {
                        return (!BubbleGrid.ok(c.Row, c.Column, vertSize, horSize));
                    });

                    if (gOS)
                        arr.RemoveAll((c) =>
                        {
                            return (!gOS.ContainID(c.ID));
                        });
                }
            };
            cAction(featuredCells);
            cAction(overlays);
            SetAsDirty();
        }

        public void IncBackGround()
        {
            backGroundNumber++;
            Save();
        }

        public void DecBackGround()
        {
            backGroundNumber--;
            Save();
        }

        private float RoundToFloat(float val, float delta)
        {
            int vi = Mathf.RoundToInt(val / delta);
            return (float)vi * delta;
        }

        private void RemoveCellData(List<CellData> cdl, CellData cd)
        {
            if (cdl != null) cdl.RemoveAll((c) => { return ((cd.Column == c.Column) && (cd.Row == c.Row)); });
        }

        /// <summary>
        /// Remove celldata overlay -> disabled
        /// </summary>
        /// <param name="cd"></param>
        public void RemoveCellData(CellData cd)
        {
            RemoveCellData(overlays, cd);
            RemoveCellData(featuredCells, cd);
        }

        private bool ContainCellData(List<CellData> lcd, CellData cd)
        {
            if (lcd == null || cd == null) return false;
            foreach (var item in lcd)
            {
                if ((item.Row == cd.Row) && (item.Column == cd.Column)) return true;
            }
            return false;
        }
    }
}
