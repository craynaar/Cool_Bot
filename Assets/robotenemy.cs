﻿using UnityEngine;
using System.Collections;

public class robotenemy : MonoBehaviour{ 
    private CharacterController enemycontroller;
    private Vector3 moveDirection = Vector3.zero;
    private float gravity = 20f;
    public GameObject route;
    public Animator ani;
    private bool HasSeenPlayer = false;
    public BoxCollider punchCollider;
    public BoxCollider inRangeCollider;
    private bool isAttacking = false;
    private Transform player;
    private NPCHealth healthsys;
    //routes will always have an null transform at the end of the array
    private Transform[] routes;
    public Transform armature;
    public float speed;
    private int checkpoint = 0;
    private Vector3 explosionPoint;
    private float force=0;
    private Vector3 trajectroy;
    private bool inTheAir = false;
	// Use this for initialization
	void Start () {
        healthsys = GetComponent<NPCHealth>();
        enemycontroller = GetComponent<CharacterController>();
        routes = route.GetComponentsInChildren<Transform>();
        shiftAll();
    }
	
	// Update is called once per frame
	void Update () {
        moveDirection = Vector3.zero;




        if (!inTheAir)
        {

            if (!HasSeenPlayer && !ani.GetBool("dead"))
            {
                if (Vector3.Distance(enemycontroller.transform.position, routes[checkpoint].position) < 2)
                    setWayPoint();
                //print(checkpoint);
                Vector3 newcheckp = new Vector3(routes[checkpoint].position.x, 0f, routes[checkpoint].position.z);

                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.position,routes[checkpoint].position), .1f * Time.deltaTime);
                transform.LookAt(routes[checkpoint]);

                transform.position = Vector3.MoveTowards(enemycontroller.transform.position, newcheckp, 4.7f * Time.deltaTime);

            }
            else if (ani.GetBool("dead")) { } //---------------------------------------------- this is unoptomised
            else if (ani.GetCurrentAnimatorStateInfo(0).IsName("running") && ((float)Vector3.Distance(transform.position, player.position) > 4f))
            // && !ani.GetCurrentAnimatorStateInfo(0).IsName("explode"))
            {
                transform.position = Vector3.MoveTowards(enemycontroller.transform.position, player.position, 10f * Time.deltaTime);
                transform.LookAt(player);

                ani.SetBool("closeEnough", false);
            }
            else
            {
                if (!((float)Vector3.Distance(transform.position, player.position) > 4f))
                    ani.SetBool("closeEnough", true);
                else ani.SetBool("closeEnough", false);
            }
            moveDirection.y -= gravity * Time.deltaTime;
            enemycontroller.Move(moveDirection);
        }
        else if (inTheAir) 
        {
            //print("triggerrerd");
           // transform.position = Vector3.MoveTowards(enemycontroller.transform.position, trajectroy, force * 200f * Time.deltaTime);
            enemycontroller.Move(trajectroy * force * 2);
            trajectroy.y -= gravity * Time.deltaTime;
        }   
        else if (!enemycontroller.isGrounded) 
        {
            trajectroy.y -= gravity;
            transform.position = Vector3.MoveTowards(enemycontroller.transform.position, trajectroy, force * 200f * Time.deltaTime);
        }


    }

    private void shiftAll() 
    {
        for (int i = 1; i < routes.Length; i++) 
        {
            routes[i - 1] = routes[i];
        }
        routes[routes.Length - 1] = null; 
    }
    private void setWayPoint() 
    {
        if (checkpoint == routes.Length - 2)
            checkpoint = 0;
        else
            checkpoint++;
    
    }

    void OnTriggerEnter(Collider other) 
    {
        if (ani.GetNextAnimatorStateInfo(0).IsName("dead")) return;



       // if (other.name == "attackCollider") 
       // {
            //GetComponent<CharacterController>().SimpleMove(new Vector3(0f,100f,0f) * 1000f);// this doesnt work
       //     GetComponent<Rigidbody>().AddExplosionForce(500f, other.GetComponentInParent<controler>().transform.position, 10f,5f);
       //     healthsys.getHit(5);
       // }
       if (other.name == "bullet(Clone)") 
       {
           healthsys.getHit(10);
           Destroy(other.gameObject, 0.1f);
        }
        if (other.name == "BlastRadius") 
        {
            Vector3 heading;
            //print("i got hit by the blast radius");
            healthsys.getHit(20);
            explosionPoint = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
            //print(Vector3.Distance(transform.position, explosionPoint));
            force =  1f / Vector3.Distance(transform.position, explosionPoint);
            //trajectroy = new Vector3(transform.position.x - explosionPoint.x, transform.position.y - explosionPoint.y, transform.position.z - explosionPoint.z);
            heading = (transform.position - explosionPoint);
            trajectroy = heading / heading.magnitude;
            trajectroy.y += 3f;
            //ani.SetTrigger("explode");
            inTheAir = true;
            StartCoroutine("flying");
            transform.LookAt(explosionPoint);
            //Destroy(other.gameObject);
        }
    }
    public IEnumerator flying() 
    {
        yield return new WaitForSeconds(0.5f);
        inTheAir = false;
    }

    public void setAttacking(bool flag) 
    {
        isAttacking = flag;
    }
    public bool getAttacking() 
    {
        return isAttacking;
    }
    public void setSeenPlayer(bool flag) 
    {
        HasSeenPlayer = flag;
    }
    public void setPlayer(Transform pla) 
    {
        player = pla;
    }
    public void setExpPos() 
    {
        gameObject.transform.position = armature.transform.position;
    
    }
}
