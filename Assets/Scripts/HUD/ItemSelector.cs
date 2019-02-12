﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    private float t;
    public float length = 1.15f, divide = 1;

    private void Update()
    {
        t = Time.time / divide;
        transform.localScale = new Vector3(Mathf.PingPong(t, length - 1) + 1, Mathf.PingPong(t, length - 1) + 1, 0);
    }
}
