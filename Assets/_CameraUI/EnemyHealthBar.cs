using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class EnemyHealthBar : MonoBehaviour
{
    
    RawImage healthBarRawImage;
    Enemy enemy;

    // Use this for initialization
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        healthBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = -(enemy.GetCurrentHealthAsPercentage() / 2f) - 0.5f;
        healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
    }
    
}
