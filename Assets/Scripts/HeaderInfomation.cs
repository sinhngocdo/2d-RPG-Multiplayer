using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInfomation : MonoBehaviourPun
{
    public TextMeshProUGUI playerName;
    public Image healthBar;
    private float maxHeathValue;


    public void Initialized(string text, int maxVal)
    {
        playerName.text = text;
        maxHeathValue = maxVal;
        healthBar.fillAmount = 1.0f;
    }


    [PunRPC]
    void UpdateHealthBar(int value)
    {
        healthBar.fillAmount = (float)value / maxHeathValue;
    }
    
}
