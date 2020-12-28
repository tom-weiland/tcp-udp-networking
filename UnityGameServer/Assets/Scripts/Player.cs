using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //players items
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    public float throwForce = 600f;

    //transform location of shoot origin
    public Transform shootOrigin;

    //player variables
    public int id;
    public string username;
    public float health = 100;
    public float maxHealth = 100f;

    //client inputs
    private bool[] inputs;

    //set variables
    [SerializeField] private float speed, jumpForce;
    [SerializeField] private float raycastDistance;

    //x and y for player moving
    private int x, y;

    //is player crouching/is player jumping
    private bool crouching, jumping;

    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInputs();
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    private void Move()
    {
        Vector3 movement = new Vector3(x, 0, y) * speed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + rb.transform.TransformDirection(movement);

        rb.MovePosition(newPosition);
    }

    private void Jump()
    {
        if (jumping && IsGrounded())
        {
            Debug.Log("jumping");
            rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        }
    }

    //sets player inputs and rotation
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void GetInputs()
    {
        if (health <= 0f)
        {
            return;
        }

        y = 0;
        x = 0;

        if (inputs[0])
        {
            y += 1;
        }

        if (inputs[1])
        {
            y -= 1;
        }

        if (inputs[2])
        {
            x -= 1;
        }

        if (inputs[3])
        {
            x += 1;
        }

        if (inputs[4])
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }

        if (inputs[5])
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, raycastDistance);
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

    public void Shoot(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
            else if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<Enemy>().TakeDamage(50f);
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            rb.isKinematic = false;
            rb.detectCollisions = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[6];
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        rb.isKinematic = true;
        rb.detectCollisions = true;
        ServerSend.PlayerRespawned(this);
    }
}


