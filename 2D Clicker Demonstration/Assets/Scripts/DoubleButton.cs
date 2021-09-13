using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleButton : MonoBehaviour
{
    public void OnClick()
    {
        DataController.Instance.gold *= 16;
    }
}
