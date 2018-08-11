using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManaOrb : GridOrb
{
    public Color[] colors;
    Color color = Color.white;
    public System.Action<ManaOrb> OnClicked;
    
    public void SetRandomColor(int max)
    {
        max = Mathf.Clamp(max, 0, colors.Length - 1);
        OrbTypeID = Random.Range(0, max);
        color = colors[OrbTypeID];
        GetComponent<Image>().color = color;
    }
    
    public void OnPointerClick()
    {
        Debug.LogFormat("Orb {0}, {1} clicked", Column, Row);
        OnClicked(this);
    }
}