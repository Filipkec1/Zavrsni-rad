using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    public static PlayerCamera instance;

    public PlayerManager player;
    public int sensitivity = 100;
    public float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;

        sensitivity = PlayerPrefs.GetInt("sensitivity");
    }

    private void Update()
    {
        if (!(GameManager.instance.shouldGameRun))
        {
            return;
        }

        if (PauseMenu.instance.inPauseMenu)
        {
            return;
        }

        Look();
        ClientSend.PlayerGunView(gameObject.transform.rotation);
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        float _mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }
}
