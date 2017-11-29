using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class TowerEnemyHealthBar : MonoBehaviour
{
    
    RawImage healthBarRawImage;
    TowerEnemy towerEnemy;

    // Use this for initialization
    void Start()
    {
        towerEnemy = GetComponentInParent<TowerEnemy>();
        healthBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = -(towerEnemy.GetCurrentHealthAsPercentage() / 2f) - 0.5f;
        healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
    }
    
}
