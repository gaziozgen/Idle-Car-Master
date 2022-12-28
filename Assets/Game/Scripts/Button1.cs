using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button1 : MonoBehaviour
{
    [SerializeField] private GameObject lockArea = null;
    [SerializeField] private GameObject infoArea = null;

    public void UpdateLock(bool locked)
    {
        lockArea.SetActive(locked);
    }

    public void UpdateInfo(bool info)
    {
        infoArea.SetActive(info);
    }
}
