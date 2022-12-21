using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{

    public Transform[] waypoints;
    public Transform[] enemies;
    public float speed = 3f;
    public float waitTime = 2f;
    public float fov = 6f;
    public float attackRange = 1f;

    private int currentWaypointIndex;
    private Animator animator;
    private float waitCounter = 0f;
    private bool isWaiting = false;
    private Transform currentEnemy;

    private EnemyManager enemyManager;
    private float attackTime = 1.5f;
    private float attackCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
        animator.SetBool("isWalking", true);
    }

    private void PatrolTask()
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

    private bool CheckIfEnemyInFOVRange()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            if(Vector3.Distance(transform.position, enemies[i].position) < fov)
            {
                currentEnemy = enemies[i];
                return true;
            }
        }
        return false;
    }

    private void GoToEnemy()
    {
        if (Vector3.Distance(transform.position, currentEnemy.position) < attackRange)
        { // we have reached the enemy
            Attack();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentEnemy.position, speed * Time.deltaTime);
            Vector3 lookDirection = currentEnemy.position - transform.position;
            lookDirection.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), speed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);
        attackCounter += Time.deltaTime;
        if(attackCounter < attackTime) // still swinging sword?
        {
            return;
        } // below here, we are done swinging sword
        enemyManager = currentEnemy.GetComponent<EnemyManager>();
        bool enemyIsDead = enemyManager.TakeHit();
        if(enemyIsDead)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", true);
            // remove the enemy from our list
            for(int i = 0; i + 1 < enemies.Length; i++)
            {
                if (enemies[i] == currentEnemy)
                {
                    enemies[i] = enemies[enemies.Length - 1];
                    break;
                }
            }
            Array.Resize(ref enemies, enemies.Length - 1);
        }
        else
        {
            // attack is done, but enemy is not dead
            attackCounter = 0f;
        }
    }

    private void Update()
    {
        bool isAttacking = CheckIfEnemyInFOVRange();
        if(isAttacking)
        {
            GoToEnemy();
        }
        else
        {
            PatrolTask();
        }
    }

}
