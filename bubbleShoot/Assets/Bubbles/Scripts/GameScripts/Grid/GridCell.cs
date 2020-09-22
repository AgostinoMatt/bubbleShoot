using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Mkey
{
    public class GridCell : MonoBehaviour, ICustomMessageTarget
    {
        #region debug
        bool debug = false;
        #endregion debug

        #region row column
        public Row<GridCell> GRow { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public List<Row<GridCell>> rows { get; private set; }
        public PFCell pfCell;
        public bool IsEvenRow { get; private set; }
        #endregion row column

        #region objects
        [SerializeField]
        public MainObject Mainobject {get; private set; }
        public OverlayObject Overlay { get; private set; }
        #endregion objects

        #region cache fields
        private CircleCollider2D coll2D;
        private SpriteRenderer sRenderer;
        private GameObject targetSelector;
        #endregion cache fields

        #region events
        public Action<GridCell> PointerDownEvent;
        public Action<GridCell> DragEnterEvent;
        #endregion events

        #region properties 
        /// <summary>
        /// Return true if mainobject and mainobject IsMatchedById || IsMatchedWithAny
        /// </summary>
        /// <returns></returns>
        public bool IsMatchable
        {
            get { return (Mainobject && Mainobject.IsMatchedById); }
        }

        /// <summary>
        /// Return true if mainobject == null
        /// </summary>
        public bool IsEmpty
        {
            get { return !Mainobject; }
        }

        /// <summary>
        /// Return true if gridcell row==0
        /// </summary>
        public bool IsTopGridcell
        {
            get { return Row == 0; }
        }

        /// <summary>
        /// Return true if gridcell row is top used for gameobects (not service)
        /// </summary>
        public bool IsTopObjectGridcell
        {
            get { return GRow.isTopObjectRow; }
        }

        /// <summary>
        /// Return true if MO IsExploidable  and !FullProtected
        /// </summary>
        /// <returns></returns>
        public bool IsExploidable
        {
            get
            {
                if (Mainobject && Mainobject.IsExploidable) return true;
                return false;
            }
        }

        public NeighBors Neighbors;//{ get; private set; }
        #endregion properties 

        private GameBoard MBoard { get { return GameBoard.Instance; } }
        private BubblesPlayer MPlayer { get { return BubblesPlayer.Instance; } }
        private BubblesGuiController MGui { get { return BubblesGuiController.Instance; } }
        private SoundMasterController MSound { get { return SoundMasterController.Instance; } }
        private GameConstructSet GcSet { get { return MPlayer.gcSet; } }
        private GameObjectsSet GoSet { get { return GcSet.GOSet; } }
        private LevelConstructSet LcSet { get { return MPlayer.LcSet; } }

        #region Objects behavior
        internal void SetMainObject(MainObjectData mObjectData, float radius, GameMode gMode)
        {
            if (mObjectData == null) { return; }
            if (!Mainobject)
            {
                Mainobject = MainObject.Create(this, mObjectData, gMode == GameMode.Play, radius, true);
              //  Debug.Log("set mo : " +Mainobject.ToString() );
                if (gMode == GameMode.Play)
                {
                    if (Mainobject == null && IsTopObjectGridcell)
                    {
                        if (coll2D) coll2D.enabled = true; // use as target for shooting
                    }
                    if (Mainobject != null && IsTopObjectGridcell)
                    {
                        if (coll2D) coll2D.enabled = false;
                    }
                }
                else
                {
                    if (coll2D) coll2D.enabled = true;
                }

            }

            else
                Mainobject.SetData(mObjectData);
            // Debug.Log("Set main object: " + ToString());
        }

        internal void SetOverlay(OverlayObjectData mObjectData)
        {
            if (mObjectData == null) return;
            if (!Overlay)
                Overlay = OverlayObject.Create(this, mObjectData);
            else
                Overlay.SetData(mObjectData);

        }

        /// <summary>
        /// Set grid cell main object to null, run CollectDelegate (if set) and completeCallBack
        /// </summary>
        /// <param name="completeCallBack"></param>
        internal void CollectShootAreaObject(Action completeCallBack, bool showPrivateScore, bool addPrivateScore, bool decProtection, int privateScore)
        {
            if (!Mainobject)
            {
                if (completeCallBack != null) completeCallBack();
                return;
            }
            Mainobject.ShootAreaCollect(completeCallBack, showPrivateScore, addPrivateScore, decProtection, privateScore);
            Mainobject = null;
            if(IsTopObjectGridcell) coll2D.enabled = true;
            if (Overlay) Overlay.ShootAreaCollect(null, showPrivateScore, addPrivateScore, decProtection, privateScore);
            Overlay = null;
        }

        internal void CollectFalledDown(Action completeCallBack, bool showPrivateScore, bool addPrivateScore, int privateScore)
        {
            if (!Mainobject)
            {
                if (completeCallBack != null) completeCallBack();
                return;
            }
            Mainobject.gameObject.layer = 2; // ignore shoot raycasting
            Mainobject.FallDownCollect(completeCallBack, showPrivateScore, addPrivateScore, privateScore);
            Mainobject = null;
            if (IsTopObjectGridcell) coll2D.enabled = true;
            if (Overlay)
            {
                Overlay.gameObject.layer = 2; // ignore shoot raycasting
                Overlay.FallDownCollect(null, showPrivateScore, addPrivateScore, privateScore);
            }
            Overlay = null;
        }

        /// <summary>
        /// Side hit from shoot bubble, it worked with destroayble mainobject
        /// </summary>
        internal void ShootHit(Action completeCallBack)
        {
            if (!Mainobject)
            {
                if (completeCallBack != null) completeCallBack();
                return;
            }
            Mainobject.ShootHit(completeCallBack);
            if (Mainobject && Mainobject.Protection <= 0) Mainobject = null;

            if (Overlay)
            {
                Overlay.ShootHit(null);
            }
        }
        #endregion Objects behavior

        /// <summary>
        ///  used by instancing for cache data
        /// </summary>
        internal void Init(int cellRow, int cellColumn, List<Row<GridCell>> rows, GameMode gMode)
        {
            Row = cellRow;
            Column = cellColumn;
            GRow = rows[cellRow];
            IsEvenRow = (cellRow % 2 == 0);
            this.rows = rows;

#if UNITY_EDITOR
            name = ToString();
#endif
            sRenderer = GetComponent<SpriteRenderer>();
            if(sRenderer) sRenderer.sortingOrder = SortingOrder.Base;
            coll2D = GetComponent<CircleCollider2D>();
            if (gMode == GameMode.Play && !IsTopObjectGridcell) DestroyImmediate(GetComponent<CircleCollider2D>());
            if (gMode == GameMode.Play) DestroyImmediate(GetComponent<SpriteRenderer>()); // 
        }

        /// <summary>
        ///  return true if main MainObjects of two cells are equal
        /// </summary>
        internal bool IsMainObjectEquals(GridCell other)
        {
            if (other == null) return false;
            if (other.Mainobject == null) return false;
            if (Mainobject == null) return false;

            return Mainobject.Equals(other.Mainobject);//Check whether the MainObject properties are equal. 
        }

        /// <summary>
        ///  cancel any tween on main MainObject object
        /// </summary>
        internal void CancelTween()
        {
            if (Mainobject)
            {
                Mainobject.CancellTweensAndSequences();
            }
            if (Overlay)
            {
                Overlay.CancellTweensAndSequences();
            }
        }

        /// <summary>
        /// DestroyImeediate MainObject, OverlayProtector, UnderlayProtector
        /// </summary>
        internal void DestroyGridObjects()
        {
            if (Mainobject) { DestroyImmediate(Mainobject.gameObject); Mainobject = null; }
            if (Overlay) { DestroyImmediate(Overlay.gameObject); Overlay = null; }
        }
    
        internal void ShowTargetSelector(bool show, Sprite sprite)
        {
            if (targetSelector && show) // need and also exist
            {
                return;
            }
            else if(!targetSelector && show && sprite) // need but not exist - create new 
            {
                targetSelector = Creator.CreateSpriteAtPosition(transform, sprite, transform.position, SortingOrder.TargetSelector).gameObject;
            }
            else if (targetSelector && !show) // not need but exist
            {
                DestroyImmediate(targetSelector);
            }
        }

        internal void ShowTargetSelector(bool show)
        {
            ShowTargetSelector(show, GoSet.selector);
        }

        #region raise events
        private void OnPointerDownEvent(GridCell gCell)
        {
            if (PointerDownEvent != null) PointerDownEvent(gCell);
        }

        private void OnDragEnterEvent(GridCell gCell)
        {
            if (DragEnterEvent != null) DragEnterEvent(gCell);
        }
        #endregion raise events

        #region touchbehavior only for construct
        public void PointerDown(TouchPadEventArgs tpea)
        {
            if (GameBoard.gMode == GameMode.Edit)
            {
                OnPointerDownEvent(this);
            }
        }

        public void Drag(TouchPadEventArgs tpea)
        {

        }

        public void DragBegin(TouchPadEventArgs tpea)
        {

        }

        public void DragDrop(TouchPadEventArgs tpea)
        {

        }

        public void DragEnter(TouchPadEventArgs tpea)
        {
            if (GameBoard.gMode == GameMode.Edit)
            {
                Debug.Log("drag enter " + ToString());
                DragEnterEvent(this);
            }
        }

        public void DragExit(TouchPadEventArgs tpea)
        {

        }

        public void PointerUp(TouchPadEventArgs tpea)
        {

        }

        public GameObject GetDataIcon()
        {
            return gameObject;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
        #endregion touchbehavior only for construct

        #region override 
        public override string ToString()
        {
            return "cell : [ row: " + Row + " , col: " + Column + "]";
        }
        #endregion override

    }

  
}