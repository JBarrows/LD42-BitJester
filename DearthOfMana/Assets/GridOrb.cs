using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class GridOrb : UIBehaviour
{
    public int Column { get; set; }
    public int Row { get; set; }


    public int OrbTypeID { get; set; }
}