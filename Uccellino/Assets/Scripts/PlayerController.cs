﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    [SerializeField]
    float speed, minSpeed, acceleration;
    Renderer renderSeed;

    public  float maxSpeed, jumpSpeed, slowSpeed;

    public bool grounded = true;
    public bool picoteando = false;
    public bool isReadyToPick = false;

    Vector3 forward, rigth;
    Rigidbody rigid;


    public int count = 0;
    public SpriteRenderer[] slots;
    public GameObject flowerPrefab;
    public GameObject[] flowerArray;
    internal int currentSlot, randomPickFlower;

    internal  Transform floorChild;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        // jumpSpeed = 3;
        slowSpeed = 1.5f;
        speed = 0f;
        maxSpeed = 7f;
        minSpeed = 0f;
        acceleration = 0.2f;
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        rigth = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    void Update()
    {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                Move();
            }

            if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
            {
                Decelerate();
            }

            if (Input.GetButtonDown("Jump") && transform.position.y < 0.6)
            {
                grounded = false;
                rigid.AddForce(Vector3.up * jumpSpeed * 100);
            }

            if(Input.GetButtonDown("Fire1") && grounded){
                picoteando = true;
                Invoke("FinishPico", .75f);
            }

            if (Input.GetButtonDown("Fire2"))
            {
            LeaveOnePerTime();
            }
    }

    void Move()
    {
        if(picoteando) return;
        if (speed <= maxSpeed)
        {
            speed += acceleration;
        }

        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 rigthMovement = rigth * speed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
        Vector3 upMovement = forward * speed * Time.deltaTime * Input.GetAxisRaw("Vertical");


        Vector3 heading = Vector3.Normalize(rigthMovement + upMovement);

        transform.position += rigthMovement; // movement happens
        transform.position += upMovement; // movement happens
    }

    void Decelerate()
    {
        while (speed >= minSpeed)
        {
            speed -= acceleration;
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                break;
            }
        }
        speed = Mathf.Clamp(speed, 0f, maxSpeed);
    }
    private void OnTriggerStay(Collider other)
    { 
        if (other.gameObject.tag == "Flower")
        {
            if (Input.GetButtonDown("Fire1") && (isReadyToPick))
            {
                slots[currentSlot].sprite = other.GetComponentInChildren<SpriteRenderer>().sprite;

                other.gameObject.SetActive(false);
                maxSpeed -= slowSpeed;
                jumpSpeed--;
                currentSlot++;
              
            }
        }

        if(other.gameObject.tag == "Seed")
        {
            IsFlower isFlower = other.GetComponent<IsFlower>();
            if (Input.GetButtonDown("Fire1"))
            {
                if (isFlower.flower)
                {
                    randomPickFlower = Random.Range(0, 4);
                    Debug.Log(randomPickFlower);
                    var flowerInGround = Instantiate(flowerArray[randomPickFlower], transform.position + new Vector3(0f, 0.0f, 0), Quaternion.identity);
                    while (flowerInGround.transform.position.y <= 0.5)
                    {
                        flowerInGround.transform.Translate(Vector3.up * 0.05f);
                    }
                    flowerInGround.transform.parent = null;
                    isReadyToPick = false;
                }
                other.gameObject.SetActive(false);
            }
        }

        if(other.gameObject.CompareTag("floor") && transform.position.y < 0.5){
            grounded = true;
        }

        if(other.gameObject.CompareTag("Water")) {
            for (int i = currentSlot - 1; i >= 0; i--)
            {
                var flowerInGround = Instantiate(slots[i], transform);
                // flowerInGround.gameObject.transform.localScale = new Vector3(1, 1, 1);

                flowerInGround.gameObject.SetActive(true);
                flowerInGround.gameObject.transform.parent = null;


                slots[i].sprite = null;
                maxSpeed += slowSpeed;
                jumpSpeed++;

            }
            currentSlot = 0;
        }
    }

    void FinishPico(){
        picoteando = false;
    }

    void LeaveOnePerTime()
    {
        var flowerInGround = Instantiate(flowerPrefab, transform.position + new Vector3(0f, 0.5f, 0), Quaternion.identity);
        flowerInGround.GetComponentInChildren<SpriteRenderer>().sprite = slots[currentSlot - 1].sprite;
        flowerInGround.transform.parent = null;
        slots[currentSlot - 1].sprite = null;
        count --;
        maxSpeed += slowSpeed;
        jumpSpeed ++;
        currentSlot --;
        Debug.Log("SAco");
    }

}
