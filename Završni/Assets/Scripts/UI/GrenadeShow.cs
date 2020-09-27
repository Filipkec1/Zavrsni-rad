using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeShow : MonoBehaviour
{
    public PlayerManager playerManager;

    public Image grenadeIconFirst;
    public Image grenadeIconSecond;
    public Image grenadeIconThird;

    public Color emtpyColor = new Color(0f, 0f, 0f, 0.14f);
    public Color fullColor = new Color(1f, 1f, 1f, 1f);

    private void Awake()
    {
        playerManager.GetComponent<PlayerManager>();
        grenadeIconFirst.GetComponent<Image>();
        grenadeIconSecond.GetComponent<Image>();
        grenadeIconThird.GetComponent<Image>();
    }

    private void Start()
    {
        grenadeIconFirst.color = emtpyColor;
        grenadeIconSecond.color = emtpyColor;
        grenadeIconThird.color = emtpyColor;
    }

    private void Update()
    {
        switch (playerManager.itemCount)
        {
            case 0:
                grenadeIconFirst.color = emtpyColor;
                grenadeIconSecond.color = emtpyColor;
                grenadeIconThird.color = emtpyColor;
                break;
            case 1:
                grenadeIconFirst.color = fullColor;
                grenadeIconSecond.color = emtpyColor;
                grenadeIconThird.color = emtpyColor;
                break;
            case 2:
                grenadeIconFirst.color = fullColor;
                grenadeIconSecond.color = fullColor;
                grenadeIconThird.color = emtpyColor;
                break;
            case 3:
                grenadeIconFirst.color = fullColor;
                grenadeIconSecond.color = fullColor;
                grenadeIconThird.color = fullColor;
                break;
        }
    }
}
