using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    public GameObject hpBar;
    private Image hpBarImg;
    public GameObject shieldBar;
    private Image shieldBarImg;

    void Start()
    {
        hpBarImg = hpBar.GetComponent<Image>();
        shieldBarImg = shieldBar.GetComponent<Image>();

        hpBarImg.fillAmount = 0.3f;
        shieldBarImg.fillAmount = 0.3f;


      
            
    }

    void Update()
    {
        Vector2 pos= shieldBar.GetComponent<RectTransform>().position;
        float posX = shieldBar.GetComponent<RectTransform>().position.x;
        posX = hpBar.GetComponent<RectTransform>().position.x;
        shieldBar.GetComponent<RectTransform>().position = new Vector2(posX, pos.y);
    }
}
