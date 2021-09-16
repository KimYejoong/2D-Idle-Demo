using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class InstantText : MonoBehaviour
{
    private Text _myText;
    private float _lifetime;

    private void Awake()
    {
        _myText = GetComponentInChildren<Text>();
    }

    public void Initialize(string content, float lifetime, TextAnchor textAnchor)
    {
        _lifetime = lifetime;
        _myText.text = content;
        _myText.alignment = textAnchor;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float offset = Time.deltaTime / _lifetime;
        for (float f = 1f; f >= 0; f -= offset)
        {
            Color c = _myText.color;
            c.a = f;
            _myText.color = c;

            yield return null;
        }
        UIManager.Instance.ReturnObject(this);
    }
}
