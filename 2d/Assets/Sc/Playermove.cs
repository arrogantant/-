using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove : MonoBehaviour
{


    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    



    CapsuleCollider2D capcollider;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capcollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }



    void Update()
    {
        
        //jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isjump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isjump", true);
            PlaySound("JUMP");


        }
        if (Input.GetButtonUp("Horizontal"))
        {

            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("iswork", false);
        else
            anim.SetBool("iswork", true);

        //damage

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //move
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("tile"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isjump", false);
            }

        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {     //attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
                onDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //점수
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;
            //아이템 사라지기
            collision.gameObject.SetActive(false);
            PlaySound("ITEM");

        }
        else if (collision.gameObject.tag == "Finish")
        {
            //다음스테이지
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 100;
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        monster enemyMove = enemy.GetComponent<monster>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK");

    }
    void onDamaged(Vector2 targetPos)
    {
        //체력다운
        gameManager.HealthDown();

        gameObject.layer = 10;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        anim.SetTrigger("DoDamaged");
        Invoke("OffDamaged", 3);
        PlaySound("DAMAGED");


    }

    void OffDamaged()
    {
        gameObject.layer = 9;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }


    public void OnDie()
    { //죽음 effect(외적인거)

        //Sprite Alpha : 색상 변경 
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Sprite Flip Y : 뒤집어지기 
        spriteRenderer.flipY = true;

        //Collider Disable : 콜라이더 끄기 
        capcollider.enabled = false;

        //Die Effect Jump : 아래로 추락(콜라이더 꺼서 바닥밑으로 추락함 )
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        PlaySound("DIE");




    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            


        }
        audioSource.Play();
    }
}
