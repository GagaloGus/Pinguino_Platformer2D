using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        LevelManager.instance.ChangeScene(sceneName);
    }
}
