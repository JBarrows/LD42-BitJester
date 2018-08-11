using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GameBoard : UIBehaviour {
    [SerializeField] private ManaOrb orbTemplate;
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 8;
    RectTransform _rctTrnsfrm;
    Vector2Int mouseDownSquare;

    public int NumColors { get; set; }
    public GridOrb[,] GridOrbs { get; set; }

    public RectTransform rectTransform
    {
        get
        {
            if (_rctTrnsfrm == null)
                _rctTrnsfrm = transform as RectTransform;
            return _rctTrnsfrm;
        }
    }

    public float OrbSize { get { return rectTransform.rect.width / width; } }

    internal ManaOrb[] GetNeighbours(int column, int row)
    {
        throw new NotImplementedException();
    }

    public static GameBoard Instance { get; internal set; }

    // Use this for initialization
    private void Start () {
        if (Instance != null) Destroy(Instance);
        Instance = this;
        NumColors = 3;
        Generate(width, height);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Generate(int width, int height)
    {
        GridOrbs = new ManaOrb[width, height];

        for (int col = 0; col < width; col++)
        {
            for (int row = 0; row < height; row++)
            {
                CreateOrb(col, row);
            }
        }
    }

    private void CreateOrb(int col, int row)
    {
        //Create orb
        ManaOrb orb = Instantiate<ManaOrb>(orbTemplate, this.transform);
        orb.SetRandomColor(NumColors);
        orb.Column = col;
        orb.Row = row;
        PlaceOrb(col, row, orb);
        //Add orb event
        orb.OnClicked += OnOrbClick;
        //Add orb to grid
        GridOrbs[col, row] = orb;
    }

    private void PlaceOrb(int col, int row, GridOrb orb)
    {
        //Set orb transform
        RectTransform orbRect = orb.GetComponent<RectTransform>();
        orbRect.sizeDelta = new Vector2(OrbSize, OrbSize);
        orbRect.localPosition = CoordinatesToLocal(col, row);
    }

    private void OnOrbClick(GridOrb orb)
    {
        //find connected orbs
        List<GridOrb> orbs = new List<GridOrb>();
        orbs = GetConnectedOrbs(orb, orbs);
        if (orbs.Count >= 3)
        {
            //TODO: Kill some orbs

            //Destroy remaining connected orbs
            foreach (ManaOrb connectedOrb in orbs)
                Destroy(connectedOrb.gameObject);

            //Fill gaps
            StartCoroutine(DropOrbs());
        }
    }

    private IEnumerator DropOrbs()
    {
        yield return new WaitForSeconds(0.1f);
        //Go row by row
        for (int row = 0; row < height; row++)
        {
            bool fillInRow = false;
            for (int col = 0; col < width; col++)
            {
                if (GridOrbs[col, row] == null)
                {
                    fillInRow = true;
                    //Find next droppable orb above
                    int y = row + 1;
                    while (GridOrbs[col, row] == null)
                    {
                        if (y == height)//Top of the board reached
                        {
                            //Generate new orb
                            CreateOrb(col, row);
                        }
                        else if (GridOrbs[col, y] != null && GridOrbs[col, y].GetType() != typeof(DeadOrb))
                        {
                            MoveOrb(col, y, col, row);
                        }
                        else
                        {
                            //No orb found: increment
                            y++;
                        }
                    }
                }
            }
            if (fillInRow)
            {
                //Wait before moving on to find the next row to fill
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void MoveOrb(int oldCol, int oldRow, int newCol, int newRow)
    {
        //Orb found, drop it in here
        GridOrb orb = GridOrbs[oldCol, oldRow];
        GridOrbs[newCol, newRow] = orb;
        orb.Column = newCol;
        orb.Row = newRow;
        PlaceOrb(newCol, newRow, orb);
        //Empty previous slot
        GridOrbs[oldCol, oldRow] = null;
    }

    private List<GridOrb> GetConnectedOrbs(GridOrb orb, List<GridOrb> orbs)
    {
        orbs.Add(orb);
        //Check above
        if (orb.Row + 1 < height)
        {
            GridOrb upOrb = GridOrbs[orb.Column, orb.Row + 1];
            if (!orbs.Contains(upOrb) && upOrb.OrbTypeID == orb.OrbTypeID)
            {
                //If the orb matches and is not already in the list
                //Add it and all of it's connected orbs
                orbs = GetConnectedOrbs(upOrb, orbs);
            }
        }
        //Check Right
        if (orb.Column + 1 < width)
        {
            GridOrb rightOrb = GridOrbs[orb.Column + 1, orb.Row];
            if (!orbs.Contains(rightOrb) && rightOrb.OrbTypeID == orb.OrbTypeID)
            {
                //If the orb matches and is not already in the list
                //Add it and all of it's connected orbs
                orbs = GetConnectedOrbs(rightOrb, orbs);
            }
        }
        //Check Below
        if (orb.Row > 0)
        {
            GridOrb downOrb = GridOrbs[orb.Column, orb.Row - 1];
            if (!orbs.Contains(downOrb) && downOrb.OrbTypeID == orb.OrbTypeID)
            {
                //If the orb matches and is not already in the list
                //Add it and all of it's connected orbs
                orbs = GetConnectedOrbs(downOrb, orbs);
            }
        }
        //Check Left
        if (orb.Column > 0)
        {
            GridOrb leftOrb = GridOrbs[orb.Column - 1, orb.Row];
            if (!orbs.Contains(leftOrb) && leftOrb.OrbTypeID == orb.OrbTypeID)
            {
                //If the orb matches and is not already in the list
                //Add it and all of it's connected orbs
                orbs = GetConnectedOrbs(leftOrb, orbs);
            }
        }
        return orbs;
    }

    private Vector2 CoordinatesToLocal(int col, int row)
    {
        return new Vector2((col * OrbSize) - (rectTransform.rect.width / 2), row * OrbSize);
    }
}
