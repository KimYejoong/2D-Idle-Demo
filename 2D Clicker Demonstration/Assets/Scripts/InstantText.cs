using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class InstantText : MonoBehaviour
{
    [HideInInspector]
    public Text myText;
    private float _lifetime;
    private Color _originalColor;
    private int _originalFontSize;

    private void Awake()
    {
        myText = GetComponentInChildren<Text>();
        _originalColor = myText.color;
        _originalFontSize = myText.fontSize;
    }

    public void Initialize(string content, float lifetime, TextAnchor textAnchor)
    {
        _lifetime = lifetime;
        myText.text = content;
        myText.alignment = textAnchor;
        myText.color = _originalColor;
        myText.fontSize = _originalFontSize;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float offset = Time.deltaTime / _lifetime;
        for (float f = 1f; f >= 0; f -= offset)
        {
            Color c = myText.color;
            c.a = f;
            myText.color = c;

            yield return null;
        }
        UIManager.Instance.ReturnObject(this);
    }
}
