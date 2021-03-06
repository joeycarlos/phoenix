﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class EnemyHealthBar : MonoBehaviour
{
    
    RawImage healthBarRawImage;
    FlyingEnemy flyingEnemy;

    // Use this for initialization
    void Start()
    {
        flyingEnemy = GetComponentInParent<FlyingEnemy>();
        healthBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = -(flyingEnemy.GetCurrentHealthAsPercentage() / 2f) - 0.5f;
        healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
    }
    
}
