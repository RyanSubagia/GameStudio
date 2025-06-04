using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfOnAnimationEnd : MonoBehaviour
{
    public void PerformDestroy()
    {
        Destroy(gameObject);
    }
}
