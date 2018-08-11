using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManaOrb : GridOrb
{
    public Color[] colors;
    Color color = Color.white;
    public System.Action<ManaOrb> OnClicked;

    public int ColorIndex { get; set; }

    public void SetRandomColor(int max)
    {
        max = Mathf.Clamp(max, 0, colors.Length - 1);
        ColorIndex = Random.Range(0, colors.Length);
        color = colors[ColorIndex];
        GetComponent<Image>().color = color;
    }
    
    public void OnPointerClick()
    {
        Debug.LogFormat("Orb {0}, {1} clicked", Column, Row);
        OnClicked(this);
    }
}