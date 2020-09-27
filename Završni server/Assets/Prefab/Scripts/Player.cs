using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    //User
    public int id;
    public string username;

    //Other
    public Rigidbody _rb;
    public Transform shootOrigin;
    public Transform groundCheck;
    public GrapplePoint grapple;
    public WeaponHolder weaponHolder;
    public Transform playerSpawns;

    //Player input
    float _x;
    float _y;

    //Movment
    public float moveSpeed = 600f;
    public float maxSpeed = 10f;
    float _angularMovment = 0.75f;

    //Counter movment
    public float counterMovement = 0.8f;
    float _threshold = 0.01f;

    //Ground check
    public bool _isGrounded = true;
    public LayerMask groundMask;
    float _groundDistance = 0.1f;

    //Jump
    bool _readyToJump = true;
    public float jumpForce = 200f;
    public float fallMultiplier = 2.5f;
    public float lowJumoMultiplier = 2f;

    //Double jump
    public float jumpCooldown = 0.1f;
    int allowJump = 1;
    public int _jumpCounter = 0;

    //sliding and crouch
    bool _isCrouching = false;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    float _raycastDistacne = 0.4f;
    bool _canSlide = false;
    public float rampForce = 500f;

    //item
    public float throwForce = 600f;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    public bool canThrow = true;
    public float cooldownTime = 2f;

    //kill / death
    public int killCount = 0;
    public int deathCount = 0;

    public float damage = 10f;

    bool[] inputs;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        playerScale = transform.localScale;

        inputs = new bool[6];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {

        if (!(GameManager.gameManager.shouldGameRun))
        {
            return;
        }

        if (health <= 0f)
        {
            return;
        }

        if(killCount >= GameManager.gameManager.killGoal)
        {
            GameManager.gameManager.EndGame(this);
        }

        PlayerInput();
        GroundCheck();
        GravityCheck();
        MovePlayer();
        CrouchCommand();
        StopPlayer();
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

    void PlayerInput()
    {

        if (health <= 0)
        {
            return;
        }


        if (inputs[0])
        {
            _x = 1f;
        }

        if (inputs[1])
        {
            _x = -1f;
        }

        if (inputs[2])
        {
            _y = 1f;
        }

        if (inputs[3])
        {
            _y = -1f;
        }

        if (inputs[5])
        {
            _isCrouching = true;
        }
        else
        {
            _isCrouching = false;
        }
    }

    void MovePlayer()
    {

        if (health <= 0) {
            return;
        }

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x;
        float yMag = mag.y;

        CounterMovement(_x, _y, mag);

        if (inputs[4])
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

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
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

        float magnitue = _rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
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

    void GravityCheck()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.velocity.y > 0 && !inputs[4])
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (lowJumoMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void StopPlayer()
    {
        Vector3 playerPositon = transform.position;

        if(_rb.velocity.magnitude < 0.01f) 
        {
            _rb.velocity = Vector3.zero;
        }
    }

    void CrouchCommand()
    {
        if (_isCrouching)
        {
            transform.localScale = crouchScale;
        }

        if (!_isCrouching && !(Physics.Raycast(transform.position, Vector3.up, gameObject.GetComponent<CapsuleCollider>().height)))
        {
            transform.localScale = playerScale;
        }

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
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

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void Shoot(Vector3 _viewDirection, bool _isShooting)
    {
        if (!(GameManager.gameManager.shouldGameRun))
        {
            return;
        }

        if (health <= 0f)
        {
            return;
        }

        weaponHolder.Shoot(_viewDirection, id, _isShooting);
    }

    public void StartGrapple(Vector3 _pos)
    {
        grapple.StartGrapple(_pos);
    }

    public void StopGrapple()
    {
        grapple.StopGrapple();
    } 

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (!(canThrow))
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
            canThrow = false;
            StartCoroutine(AllowThrow());
        }
    }

    IEnumerator AllowThrow()
    {
        yield return new WaitForSeconds(cooldownTime);
        canThrow = true;
    }

    public void TakeDamage(float _damage, int _id)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            PlayerNewPosition();

            if(id != _id)
            {
                Server.clients[_id].player.killCount++;
            }
            else
            {
                Server.clients[_id].player.killCount--;
            }
            deathCount++;

            ServerSend.SetKD(id, killCount, deathCount);
            ServerSend.SetKD(_id, Server.clients[_id].player.killCount, Server.clients[_id].player.deathCount);

            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        health = maxHealth;
        ServerSend.PlayerRespawned(this);
    }

    public void PlayerNewPosition()
    {
        int playerSpawnPicker;
        bool canAllow = true;

        do
        {
            playerSpawnPicker = Random.Range(0, playerSpawns.childCount);

            for (int i = 0; i < playerSpawns.childCount; i++)
            {
                playerSpawns.GetChild(i).GetComponent<BoxCollider>().enabled = true;
                if (playerSpawns.GetChild(i).GetComponent<IsTouching>().isTouching)
                {
                    Debug.Log("TEST");
                    break;
                }
                playerSpawns.GetChild(i).GetComponent<BoxCollider>().enabled = false;

                if (playerSpawnPicker == i)
                {
                    transform.position = playerSpawns.GetChild(i).transform.position;
                    transform.rotation = playerSpawns.GetChild(i).transform.rotation;
                    canAllow = false;
                    break;
                }
            }

        } while (canAllow);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }

    public bool AttemptPickupHeal(float _healAmount)
    {
        if(health >= maxHealth)
        {
            return false;
        }

        float toHeal = health + _healAmount;

        if(toHeal > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += _healAmount;
        }

        return true;
    }
}
