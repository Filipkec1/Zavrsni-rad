using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthShowText : MonoBehaviour
{
    public PlayerManager playerManager;

    public TextMeshProUGUI playerHealth;
    public Image iconHealth;

    private void Awake()
    {
        playerHealth.GetComponent<TextMeshPro>();
        playerManager.GetComponent<PlayerManager>();
        iconHealth.GetComponent<Image>();
    }

    private void Start()
    {
        playerHealth.text = playerManager.currentHealth.ToString();
    }

    public Color color;
    public float colorPercent;

    private void Update()
    {
        colorPercent = (255f * (playerManager.currentHealth/100f))/255f;

        float redValue = 1f - colorPercent;

        color = new Color(redValue, colorPercent, 0f);

        playerHealth.color = color;
        iconHealth.color = color;

        playerHealth.text = playerManager.currentHealth.ToString();
    }
}
