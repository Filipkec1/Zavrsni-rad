using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    //Assingables
    public Transform playerCam;
    public Transform groundCheck;
    public GrapplePoint grapple;
    public GameObject playerHUD;

    //Other
    public Rigidbody _rb;

    //Movment
    public float moveSpeed = 10f;
    public float maxSpeed = 25f;
    float _angularMovment = 0.75f;

    //Counter movment
    public float counterMovement = 0.175f;
    float _threshold = 0.01f;

    //Look and rotation
    public float sensitivity = 10f;

    //Player input
    float _x;
    float _y;

    //Ground check
    bool _isGrounded = true;
    public LayerMask groundMask;
    float _groundDistance = 0.1f;

    //Jump
    public bool _isJumping = false;
    public float jumpForce = 400f;
    public float fallMultiplier = 2.5f;
    public float lowJumoMultiplier = 2f;

    bool _readyToJump = true;
    public float jumpCooldown = 0.1f;
    int allowJump = 1;
    int _jumpCounter = 0;

    //sliding and crouch
    bool _isCrouching = false;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    float _raycastDistacne = 0.4f;
    bool _canSlide = false;
    public float rampForce = 500f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        playerScale = transform.localScale;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        OtherInputToServer();
    }

    void FixedUpdate()
    {
        if (!(GameManager.instance.shouldGameRun))
        {
            playerHUD.SetActive(false);
            return;
        }
        else
        {
            playerHUD.SetActive(true);
        }

        if (PauseMenu.instance.inPauseMenu)
        {
            return;
        }

        SendInputToServer();
        GroundCheck();
        GravityCheck();
        MovePlayer();
        CrouchCommand();
        StopPlayer();
    }

    void SendInputToServer()
    {
        _x = Input.GetAxisRaw("Horizontal");
        _y = Input.GetAxisRaw("Vertical");

        bool[] rawInput = new bool[6];

        //Up/down
        if (_x > 0)
        {
            rawInput[0] = true;
            rawInput[1] = false;
        }
        else if (_x == 0)
        {
            rawInput[0] = false;
            rawInput[1] = false;
        }
        else
        {
            rawInput[0] = false;
            rawInput[1] = true;
        }

        //Left/rightr
        if (_y > 0)
        {
            rawInput[2] = true;
            rawInput[3] = false;
        }
        else if (_y == 0)
        {
            rawInput[2] = false;
            rawInput[3] = false;
        }
        else
        {
            rawInput[2] = false;
            rawInput[3] = true;
        }

        //Jump
        if (_isJumping)
        {
            rawInput[4] = true;
        }
        else
        {
            rawInput[4] = false;
        }

        //Crouch
        if (_isCrouching)
        {
            rawInput[5] = true;
        }
        else
        {
            rawInput[5] = false;
        }

        ClientSend.PlayerMovement(rawInput);
    }

    void OtherInputToServer()
    {
        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            _isJumping = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            _isJumping = false;
        }

        //Crouch
        if (Input.GetButtonDown("Crouch"))
        {
            _isCrouching = true;
        }

        if (Input.GetButtonUp("Crouch"))
        {
            _isCrouching = false;
        }

        if (Input.GetButton("Fire1") && GameManager.instance.shouldGameRun)
        {
            ClientSend.PlayerShoot(playerCam.forward, true);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            ClientSend.PlayerShoot(playerCam.forward, false);
        }

        if (Input.GetButton("Fire2"))
        {
            ClientSend.PlayerThrowItem(playerCam.forward);
        }
    }

    void MovePlayer()
    {
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x;
        float yMag = mag.y;

        CounterMovement(_x, _y, mag);

        if (_isJumping)
        {
            JumpCommand();
        }

        //Sliding down ramp
        if (_isCrouching && _canSlide)
        {
            _rb.AddForce(Vector3.down * Time.fixedDeltaTime * rampForce);
            return;
        }

        if (_y != 0 && _x != 0)
        {
            if (_x > 0 && xMag > maxSpeed * _angularMovment) _x = 0;
            if (_x < 0 && xMag < -maxSpeed * _angularMovment) _x = 0;
            if (_y > 0 && yMag > maxSpeed * _angularMovment) _y = 0;
            if (_y < 0 && yMag < -maxSpeed * _angularMovment) _y = 0;
        }
        else
        {
            if (_x > 0 && xMag > maxSpeed) _x = 0;
            if (_x < 0 && xMag < -maxSpeed) _x = 0;
            if (_y > 0 && yMag > maxSpeed) _y = 0;
            if (_y < 0 && yMag < -maxSpeed) _y = 0;
        }

        _rb.AddForce(gameObject.transform.forward * _y * moveSpeed * Time.fixedDeltaTime);
        _rb.AddForce(gameObject.transform.right * _x * moveSpeed * Time.fixedDeltaTime);
    }

    void GroundCheck()
    {
        RampSliding();

        if (Physics.CheckSphere(groundCheck.transform.position, _groundDistance, groundMask))
        {
            _isGrounded = true;
            _readyToJump = true;
            _jumpCounter = 0;
        }
        else
        {
            _isGrounded = false;
        }
    }

    void RampSliding()
    {
        RaycastHit hit;
        Physics.Raycast(groundCheck.transform.position, Vector3.down, out hit, _raycastDistacne, groundMask);

        if (hit.collider)
        {
            Vector3 rotation = hit.collider.gameObject.transform.rotation.eulerAngles;

            if (rotation.x != 0f || rotation.z != 0f)
            {
                _canSlide = true;
            }
        }
        else
        {
            _canSlide = false;
        }
    }

    public void JumpCommand()
    {
        if (_isGrounded && _readyToJump)
        {
            _rb.AddForce(Vector3.up * jumpForce);

            _readyToJump = false;
        }

        if (!_isGrounded && _jumpCounter < allowJump && _readyToJump)
        {
            Vector3 vel = _rb.velocity;
            if (_rb.velocity.y < 0.5f)
            {
                _rb.velocity = new Vector3(vel.x, 0, vel.z);
            }
            _rb.AddForce(Vector3.up * jumpForce);

            _readyToJump = false;
            _jumpCounter++;
        }
        StartCoroutine(ResetJump());
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpCooldown);

        _readyToJump = true;
    }


    void CrouchCommand()
    {
        if (_isCrouching)
        {
            transform.localScale = crouchScale;
        }

        if(!_isCrouching && !(Physics.Raycast(transform.position, Vector3.up, gameObject.GetComponent<CapsuleCollider>().height)))
        {
            transform.localScale = playerScale;
        }
    }

    void GravityCheck()
    {

        if(_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if(_rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (lowJumoMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!_isGrounded)
        {
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > _threshold && Math.Abs(x) < 0.05f || (mag.x < -_threshold && x > 0) || (mag.x > _threshold && x < 0))
        {
            _rb.AddForce(moveSpeed * transform.right * Time.fixedDeltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > _threshold && Math.Abs(y) < 0.05f || (mag.y < -_threshold && y > 0) || (mag.y > _threshold && y < 0))
        {
            _rb.AddForce(moveSpeed * transform.forward * Time.fixedDeltaTime * -mag.y * counterMovement);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = gameObject.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(_rb.velocity.x, _rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        //vraća duljinu vektora 
        float magnitue = _rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    void StopPlayer()
    {
        Vector3 playerPositon = transform.position;

        if (_rb.velocity.magnitude < 0.01f)
        {
            _rb.velocity = Vector3.zero;
        }
    }
}
