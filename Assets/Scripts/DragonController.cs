using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    GameObject player;
    ShootController gameController;
    public AudioController audioController;
    public AudioClip dragonScreamClip;
    public AudioClip[] audioClips = new AudioClip[4];

    Animator animator;
    bool isDragonWalking;
    public GameObject FinalTarget;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerChar");
        gameController = player.GetComponent<ShootController>();
        animator = this.GetComponent<Animator>();

        //play intimidating song, and then in phase 2 play triumphant song

        if(!gameController.isPhase1Done)
        {
            //play scream at start
            StartCoroutine(MakeScream());

            //waits until scream is done before walking

            //sometimes make the dragon walk

            //start dragon walking after scream
            InvokeRepeating("MakeDragonWalk", 4.0f, 10.0f);
            //stop the dragon from walking 
            InvokeRepeating("MakeDragonIdle", 8.0f, 10.0f);
            //the last two numbers should stay the same in this case (methodName, startingTime, intervalTime)
        }

        if(gameController.isPhase1Done)
        {
            SetupPhase2();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetBool("isDead"))
        {
            animator.SetBool("isWalking", false);
            animator.Play("Die");
        }

        if(animator.GetBool("isHit"))
        {
            animator.SetBool("isWalking", false);
            animator.Play("GetHit");
            animator.SetBool("isHit", false);
        }

        
    }

    private void FixedUpdate()
    {

        //sometimes follow the player
        if (isDragonWalking && !gameController.isPhase1Done && !animator.GetBool("isScreaming") && !animator.GetBool("isDead") && !animator.GetBool("isHit"))
        {
            //move dragon towards the player

            animator.SetBool("isWalking", true);

            //look at the player while maintaining the verticality
            this.GetComponent<Transform>().LookAt(new Vector3(player.transform.position.x, this.transform.rotation.y, player.transform.position.z));

            //follow the player
            this.transform.Translate(new Vector3(0, 0, 0.05f), Space.Self);

            //if disance between dragon and player is low, play attack animation
            float distance = Vector3.Distance(player.transform.position, this.transform.position);
            if (distance <= 25.0f)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
            }
        }

        if (gameController.isPhase1Done && !animator.GetBool("isScreaming") && !animator.GetBool("isDead"))
        {
            //run away from the player in phase 2

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);

            //look away from the player
            this.GetComponent<Transform>().LookAt(new Vector3((2 * this.transform.position.x - player.transform.position.x), this.transform.rotation.y, (2 * this.transform.position.z - player.transform.position.z)));
            //(2 * this.transform.position.x - player.transform.position.x)
            
            //run away
            this.transform.Translate(new Vector3(0, 0, 0.01f), Space.Self);
        }
    }


    void MakeDragonWalk()
    {
        if(!gameController.isPhase1Done)
        {
            isDragonWalking = true;


            animator.SetBool("isAttacking", false);
        }
        
    }

    void MakeDragonIdle()
    {
        if (!gameController.isPhase1Done)
        {
            isDragonWalking = false;
            animator.SetBool("isWalking", false);


            animator.SetBool("isAttacking", false);
        }
    }

    IEnumerator MakeScream()
    {
        animator.SetBool("isScreaming", true);

        yield return new WaitForSeconds(2.0f);
        //play scream sound
        audioController.newSoundtrack(dragonScreamClip);

        audioController.PlaySimultaneously(audioClips[0]);

        yield return new WaitForSeconds(2.0f);
        animator.SetBool("isScreaming", false);

        yield return new WaitForSeconds(4.0f);
        audioController.newSoundtrack(audioClips[1]);
    }

    public void SetupPhase2()
    {
        //make dragon have 1 target 
        //StartCoroutine(ShowFinalTarget());

        //make sure collider is here
        GameObject dragonCollider = this.transform.Find("Root/DragonColliderPhase2").gameObject;
        dragonCollider.SetActive(true);
    }

    IEnumerator ShowFinalTarget()
    {
        //shows the final target after 4 seconds (when the player is full height)
        yield return new WaitForSeconds(3);
        FinalTarget.SetActive(true);
    }
}
