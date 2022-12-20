using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{

    public Transform[] waypoints;
    public float speed = 3f;
    public float waitTime = 2f;

    private int currentWaypointIndex;
    private Animator animator;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        animator.SetBool("isWalking", true);
    }

    // Update is called once per frame
    void Update()
    {
        if(isWaiting)
        {
            waitCounter += Time.deltaTime;
            if(waitCounter < waitTime)
            {
                return;
            }
            isWaiting = false;
            animator.SetBool("isWalking", true);
        }
        Transform currentWaypoint = waypoints[currentWaypointIndex];
        if(Vector3.Distance(transform.position, currentWaypoint.position) < 0.01f)
        { // we got to a waypoint...
            transform.position = currentWaypoint.position;
            waitCounter = 0f;
            isWaiting = true;
            animator.SetBool("isWalking", false);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime);
            //transform.LookAt(currentWaypoint.position); // turns too suddenly
            Vector3 lookDirection = currentWaypoint.position - transform.position;
            lookDirection.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), speed * Time.deltaTime);
        }
    }
}
