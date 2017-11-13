using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RawImage))]
public class PlayerJumpBar : MonoBehaviour
{
    
    RawImage jumpBarRawImage;
    Player player;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        jumpBarRawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        float xValue = -(player.GetCurrentJumpChargeAsPercentage() / 2f) - 0.5f;
        jumpBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
    }
    
}
