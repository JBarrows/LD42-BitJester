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

    public ManaOrb[,] ManaOrbs { get; set; }

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
        Generate(width, height);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Generate(int width, int height)
    {
        ManaOrbs = new ManaOrb[width, height];

        for (int col = 0; col < width; col++)
        {
            for (int row = 0; row < height; row++)
            {
                //Create orb
                ManaOrb orb = Instantiate<ManaOrb>(orbTemplate, this.transform);
                orb.SetRandomColor(3);
                orb.Column = col;
                orb.Row = row;
                //Set orb transform
                RectTransform orbRect = orb.GetComponent<RectTransform>();
                orbRect.sizeDelta = new Vector2(OrbSize, OrbSize);
                orbRect.localPosition = CoordinatesToLocal(col, row);
                //Add orb event
                orb.OnClicked += OnOrbClick;
                //Add orb to grid
                ManaOrbs[col, row] = orb;
            }
        }
    }

    private void OnOrbClick(ManaOrb orb)
    {
        //find connected orbs
        List<ManaOrb> orbs = new List<ManaOrb>();
        orbs = GetConnectedOrbs(orb, orbs);
        foreach (ManaOrb connectedOrb in orbs)
            Destroy(connectedOrb.gameObject);
    }

    private List<ManaOrb> GetConnectedOrbs(ManaOrb orb, List<ManaOrb> orbs)
    {
        orbs.Add(orb);
        //Check above
        if (orb.Row + 1 < height)
        {
            ManaOrb upOrb = ManaOrbs[orb.Column, orb.Row + 1];
            if (!orbs.Contains(upOrb) && upOrb.ColorIndex == orb.ColorIndex)
            {
                //If the orb matches and is not already in the list
                //Add it and all of it's connected orbs
                orbs = GetConnectedOrbs(upOrb, orbs);
            }
        }
        //Check Right
        if (orb.Column + 1 < width)
        {
            ManaOrb rightOrb = ManaOrbs[orb.Column + 1, orb.Row];
            if (!orbs.Contains(rightOrb) && rightOrb.ColorIndex == orb.ColorIndex)
            {
                //If the orb matches and is not already in the list
                //Add it and all of it's connected orbs
                orbs = GetConnectedOrbs(rightOrb, orbs);
            }
        }
        //Check Below
        if (orb.Row > 0)
        {
            ManaOrb downOrb = ManaOrbs[orb.Column, orb.Row - 1];
            if (!orbs.Contains(downOrb) && downOrb.ColorIndex == orb.ColorIndex)
            {
                //If the orb matches and is not already in the list
                //Add it and all of it's connected orbs
                orbs = GetConnectedOrbs(downOrb, orbs);
            }
        }
        //Check Left
        if (orb.Column > 0)
        {
            ManaOrb leftOrb = ManaOrbs[orb.Column - 1, orb.Row];
            if (!orbs.Contains(leftOrb) && leftOrb.ColorIndex == orb.ColorIndex)
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
