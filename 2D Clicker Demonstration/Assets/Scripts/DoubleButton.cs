using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleButton : MonoBehaviour
{
    public void OnClick()
    {
        DataController.Gold *= 16;
    }
}
