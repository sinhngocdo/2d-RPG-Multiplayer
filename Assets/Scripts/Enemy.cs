using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
    public string enemyName;
    public float moveSpeed;
    public int currenHP;
    public int maxHP;
    public float chaseRange;
    public float attackRange;
    private PlayerController targetPlayer;
    public float playerDetectRate;
    private float lastPlayerDetectTime;
    public string objectToSpawnOnDeath;
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public HeaderInfomation healthBar;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    public Rigidbody2D rb2d;


     

    private void Start()
    {
        healthBar.Initialized(enemyName, maxHP);

    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (targetPlayer != null)
        {
            float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            float face = targetPlayer.transform.position.x - transform.position.x;

            if (face > 0)
            {
                photonView.RPC("FlipRight", RpcTarget.All);
            }
            else
            {
                photonView.RPC("FlipLeft", RpcTarget.All);
            }

            // if we're able to attack, do so
            if (dist < attackRange && Time.time - lastAttackTime >= attackRate)
            {
                Attack();
            }
            // otherwise, do we move after the player?
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                rb2d.velocity = dir.normalized * moveSpeed;
                anim.SetBool("Walk", true);
            }
            else
            {
                rb2d.velocity = Vector2.zero;
                anim.SetBool("Walk", false);
            }

        }

        DeteckPlayer();

    }







    [PunRPC]
    void FlipRight()
    {
        spriteRenderer.flipX = false;
    }

    [PunRPC]
    void FlipLeft()
    {
        spriteRenderer.flipX = true;
    }


    void Attack()
    {
        anim.SetTrigger("Attack");
        lastAttackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", targetPlayer.photonPlayer, damage);
    }


    void DeteckPlayer()
    {
        if (Time.time - lastPlayerDetectTime > playerDetectRate)
        {
            lastPlayerDetectTime = Time.time;
            //loop through all players
            foreach (PlayerController player in GameManager.instance.players)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                    {
                        targetPlayer = null;
                        anim.SetBool("Walk", false);
                        rb2d.velocity = Vector2.zero;
                    }
                }
                else if(dist < chaseRange)
                {
                    if (targetPlayer == null)
                    {
                        targetPlayer = player;
                    }
                }
            }
        }
    }


    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        currenHP -= damageAmount;
        healthBar.photonView.RPC("UpdateHealthBar", RpcTarget.All, currenHP);
        if (currenHP <= 0)
        {
            //die
            Die();
        }
        else
        {
            photonView.RPC("FlashDamage", RpcTarget.All);
        }
    }

    [PunRPC]
    void FlashDamage()
    {
        StartCoroutine(DamageFlash());
        IEnumerator DamageFlash()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = Color.white;
        }
    }



    void Die()
    {
        //drop an item after death
        if (objectToSpawnOnDeath != string.Empty)
        {
            PhotonNetwork.Instantiate(objectToSpawnOnDeath, transform.position, Quaternion.identity);
        }
        //destroy enemy
        PhotonNetwork.Destroy(gameObject);
    }



}
