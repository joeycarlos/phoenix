using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class PlayerEnergyBar : MonoBehaviour
{
    
    RawImage energyBarRawImage;
    Player player;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        energyBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = -(player.GetCurrentEnergyAsPercentage() / 2f) - 0.5f;
        energyBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
    }
    
}
