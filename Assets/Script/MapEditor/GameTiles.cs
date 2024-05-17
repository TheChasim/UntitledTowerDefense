using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTiles : MonoBehaviour
{
    private Color originalColor;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer SlowingRenderer;
    public SpriteRenderer WallRenderer;
    public SpriteRenderer DamagingRenderer;


    public bool IsBloced = false;
    Color ColorBloced = Color.black;
    public bool IsSlowing = false;
    Color ColorSlowing = Color.blue;
    public bool IsDamaging = false;
    Color ColorDamaging = Color.red;

    internal void SetComponent()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    internal void TurnGrey(float alphaValue)
    {
        Color currentColor = originalColor;
        Color newColor = new Color(currentColor.grayscale, currentColor.grayscale, currentColor.grayscale, 0.5f);
        spriteRenderer.color = newColor;
    }

    internal void TurnBloced()
    {
        IsBloced =! IsBloced;
        WallRenderer.enabled = IsBloced;
    }

    internal void TurnSlow()
    {
        IsSlowing =! IsSlowing;
        SlowingRenderer.enabled = IsSlowing;
    }

    internal void TurnDamaging()
    {
        IsDamaging =! IsDamaging;
        DamagingRenderer.enabled = IsDamaging;
    }
}
