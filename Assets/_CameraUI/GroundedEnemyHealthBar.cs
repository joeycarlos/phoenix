using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class GroundedEnemyHealthBar : MonoBehaviour
{
    
    RawImage healthBarRawImage;
    GroundedEnemy groundedEnemy;

    // Use this for initialization
    void Start()
    {
        groundedEnemy = GetComponentInParent<GroundedEnemy>();
        healthBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = -(groundedEnemy.GetCurrentHealthAsPercentage() / 2f) - 0.5f;
        healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
    }
    
}
