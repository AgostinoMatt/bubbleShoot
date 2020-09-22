using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    /// <summary>
    /// Get neighborns for gridcell 
    /// </summary>
    public class NeighBors
    {
        public GridCell Main { get; private set; }
        public GridCell Left { get; private set; }
        public GridCell Right { get; private set; }
        public GridCell TopLeft { get; private set; }
        public GridCell TopRight { get; private set; }
        public GridCell BottomLeft { get; private set; }
        public GridCell BottomRight { get; private set; }
        public List<GridCell> Cells { get; private set; }
        public List<GridCell> EqualIDCells { get; private set; }
        public List<GridCell> EmptyCells { get; private set; }
        public List<GridCell> NotEmptyCells { get; private set; }


        /// <summary>
        /// Create NeighBorns  cells arrays: EqualIdCells (for id), EmptyCells, NotEmptyCells
        /// </summary>
        /// <param name="main"></param>
        /// <param name="id"></param>
        public NeighBors(GridCell main, int id)
        {
            Main = main;
            int leftCol = main.Column - 1;
            int rightCol = main.Column + 1;
            Row<GridCell> botRow = (main.Row + 1 < main.rows.Count) ? main.rows[main.Row + 1] : null;
            Row<GridCell> topRow = (main.Row - 1 >= 0) ? main.rows[main.Row - 1] : null;

            Left = main.GRow[leftCol];
            Right = main.GRow[rightCol];

            TopLeft = (topRow != null) ? topRow[(main.IsEvenRow) ? leftCol : main.Column] : null;
            TopRight = (topRow != null) ? topRow[(main.IsEvenRow) ? main.Column : rightCol] : null;

            BottomLeft = (botRow != null) ? botRow[(main.IsEvenRow) ? leftCol : main.Column] : null;
            BottomRight = (botRow != null) ? botRow[(main.IsEvenRow) ? main.Column : rightCol] : null;

            FillArrays(id);
        }

        /// <summary>
        /// Create NeighBorns  Cells array
        /// </summary>
        /// <param name="main"></param>
        /// <param name="id"></param>
        public NeighBors(GridCell main)
        {
            Main = main;
            int leftCol = main.Column - 1;
            int rightCol = main.Column + 1;
            Row<GridCell> botRow = (main.Row + 1 < main.rows.Count) ? main.rows[main.Row + 1] : null;
            Row<GridCell> topRow = (main.Row - 1 >= 0) ? main.rows[main.Row - 1] : null;

            Left = main.GRow[leftCol];
            Right = main.GRow[rightCol];

            TopLeft = (topRow != null) ? topRow[(main.IsEvenRow) ? leftCol : main.Column] : null;
            TopRight = (topRow != null) ? topRow[(main.IsEvenRow) ? main.Column : rightCol] : null;

            BottomLeft = (botRow != null) ? botRow[(main.IsEvenRow) ? leftCol : main.Column] : null;
            BottomRight = (botRow != null) ? botRow[(main.IsEvenRow) ? main.Column : rightCol] : null;

            Cells = new List<GridCell>();
            if (Left != null) Cells.Add(Left);
            if (Right != null) Cells.Add(Right);
            if (TopLeft != null) Cells.Add(TopLeft);
            if (BottomLeft != null) Cells.Add(BottomLeft);
            if (TopRight != null) Cells.Add(TopRight);
            if (BottomRight != null) Cells.Add(BottomRight);

            FillArrays();
        }

        public bool Contain(GridCell gCell)
        {
            if (gCell == null) return false;
            return Cells.Contains(gCell);
        }

        private void FillArrays(int id)
        {
            Cells = new List<GridCell>();
            if (Left != null) Cells.Add(Left);
            if (Right != null) Cells.Add(Right);
            if (TopLeft != null) Cells.Add(TopLeft);
            if (BottomLeft != null) Cells.Add(BottomLeft);
            if (TopRight != null) Cells.Add(TopRight);
            if (BottomRight != null) Cells.Add(BottomRight);

            EmptyCells = new List<GridCell>();
            NotEmptyCells = new List<GridCell>();
            EqualIDCells = new List<GridCell>();

            for (int i = 0; i < Cells.Count; i++)
            {
                GridCell c = Cells[i];
                if (c)
                {
                    if (!c.IsEmpty)
                    {
                        NotEmptyCells.Add(c);
                    }
                    else
                    {
                        EmptyCells.Add(c);
                    }

                    if (c.Mainobject && (c.Mainobject.ID == id))
                    {
                        EqualIDCells.Add(c);
                    }
                }
            }
        }

        private void FillArrays()
        {
            Cells = new List<GridCell>();
            if (Left != null) Cells.Add(Left);
            if (Right != null) Cells.Add(Right);
            if (TopLeft != null) Cells.Add(TopLeft);
            if (BottomLeft != null) Cells.Add(BottomLeft);
            if (TopRight != null) Cells.Add(TopRight);
            if (BottomRight != null) Cells.Add(BottomRight);

            EmptyCells = new List<GridCell>();
            NotEmptyCells = new List<GridCell>();
            EqualIDCells = new List<GridCell>();

            for (int i = 0; i < Cells.Count; i++)
            {
                GridCell c = Cells[i];
                if (c)
                {
                    if (!c.IsEmpty)
                    {
                        NotEmptyCells.Add(c);
                    }
                    else
                    {
                        EmptyCells.Add(c);
                    }
                }
            }
        }

        public GridCell GetEmptyLeftBottom()
        {
            if (BottomLeft && BottomLeft.IsEmpty) return BottomLeft;
            if (Left && Left.IsEmpty) return Left;
            if (TopLeft && TopLeft.IsEmpty) return TopLeft;
            return null;
        }

        public GridCell GetEmptyLeftMiddle()
        {
            if (Left && Left.IsEmpty) return Left;
            if (BottomLeft && BottomLeft.IsEmpty) return BottomLeft;
            if (TopLeft && TopLeft.IsEmpty) return TopLeft;
            return null;
        }

        public GridCell GetEmptyLefTop()
        {
            if (TopLeft && TopLeft.IsEmpty) return TopLeft;
            if (Left && Left.IsEmpty) return Left;
            if (BottomLeft && BottomLeft.IsEmpty) return BottomLeft;
            return null;
        }

        public GridCell GetEmptyRightBottom()
        {
            if (BottomRight && BottomRight.IsEmpty) return BottomRight;
            if (Right && Right.IsEmpty) return Right;
            if (TopRight && TopRight.IsEmpty) return TopRight;
            return null;
        }

        public GridCell GetEmptyRightMiddle()
        {
            if (Right && Right.IsEmpty) return Right;
            if (BottomRight && BottomRight.IsEmpty) return BottomRight;
            if (TopRight && TopRight.IsEmpty) return TopRight;
            return null;
        }

        public GridCell GetEmptyRightTop()
        {
            if (TopRight && TopRight.IsEmpty) return TopRight;
            if (Right && Right.IsEmpty) return Right;
            if (BottomRight && BottomRight.IsEmpty) return BottomRight;
            return null;
        }

        public GridCell GetEmpty(bool left, bool bottom)
        {
            if (left)
            {
                if (bottom)
                    return GetEmptyLeftBottom();
                else
                    return GetEmptyLeftMiddle();
            }
            else
            {
                if (bottom)
                    return GetEmptyRightBottom();
                else
                    return GetEmptyRightMiddle();
            }
        }

        public List<PFCell> GetNeighBorsPF()
        {
            List<PFCell> res = new List<PFCell>();
            foreach (var item in Cells)
            {
                res.Add(item.pfCell);
            }
            return res;
        }

        public void Remove(GridCell gCell)
        {
            if (Contain(gCell))
            {
                Cells.Remove(gCell);
                if (TopLeft == gCell) TopLeft = null;
                if (Left == gCell) Left = null;
                if (BottomLeft == gCell) BottomLeft = null;

                if (BottomRight == gCell) BottomRight = null;
                if (Right = gCell) Right = null;
                if (TopRight = gCell) TopRight = null;
            }
        }

        public override string ToString()
        {
            return ("All cells : " + ToString(Cells) + " ;Empty cells: " + ToString(EmptyCells) + " ; Not Empty : " + ToString(NotEmptyCells));
        }

        public string ToString(List<GridCell> list)
        {
            string res = "";
            foreach (var item in list)
            {
                res += item.ToString();
            }
            return res;
        }
    }
}