using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShootController : MonoBehaviour
{
    //this class is more of a "(player) mechanics controller" class

    public Hero playerController;
    public GameObject shootParticle;
    public GameObject hitParticle;
    public Material targetHitMaterial;
    public float scaleRate = 1.0f;
    public float finalPlayerHeight = 50.0f;

    public TextMeshProUGUI Instructions;
    public TextMeshProUGUI hitnumberUI;

    private Animator dragonAnimator;
    public GameObject DragonPrefab;
    public int dragonsToSpawn = 10;
    int totalDragonsKilled;

    int numberOfTargetsHit;
    int totalNumberOfTargets;
    public bool isPhase1Done;

    private Vector3 scaleChange;

    bool gameWon;

    // Start is called before the first frame update
    void Start()
    {
        numberOfTargetsHit = 0;
        totalNumberOfTargets = 5; //GameObject.Find("Targets").GetComponentsInChildren<Transform>().Length;
        isPhase1Done = false;
        gameWon = false;
        totalDragonsKilled = 0;

        //set the player growth rate
        scaleChange = new Vector3(scaleRate, scaleRate, scaleRate);
    }

    // Update is called once per frame
    void Update()
    {
        hitNumberUpdate();

        if (Input.GetMouseButtonDown(0))
        {
            //instatiate a ray
            //get the camera and the screen to point ray from the mouse position
            Ray theRay = Camera.main.ScreenPointToRay(Input.mousePosition); //can change to current or main

            //have a variable for when it hits
            RaycastHit rayHitInfo;

            //Check if our ray has indeed hit something
            if (Physics.Raycast(theRay, out rayHitInfo))
            {                                
                
                //Debug.Log("shot at "+ rayHitInfo.point);


                // if the mouse clicks on an object with this tag:
                if (rayHitInfo.collider.gameObject.CompareTag("Target") && !isPhase1Done)
                {
                    //Debug.Log("target hit!");

                    //instantiate the hit particle
                    GameObject particle = Instantiate(hitParticle, rayHitInfo.point, Quaternion.identity);
                    Destroy(particle, 5.0f); //destroy the particle object after 5 seconds

                    //set the dragon's animator
                    GameObject root = rayHitInfo.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;
                    dragonAnimator = root.transform.parent.gameObject.GetComponent<Animator>();

                    //change the material to show that we hit
                    rayHitInfo.collider.gameObject.GetComponent<MeshRenderer>().material = targetHitMaterial;

                    //hide instructions when the player hits the first target
                    HideInstructions();


                    //Dragon Feedback:
                    dragonAnimator.SetBool("isHit", true);
                    //TODO: could play a hit sound

                    //count the targets
                    numberOfTargetsHit++;

                    //a boolean check when it's over
                    if (numberOfTargetsHit >= totalNumberOfTargets)
                    {
                        isPhase1Done = true;
                        StartPhase2();
                        //setup phase 2 for the existing dragon
                        rayHitInfo.collider.transform.parent.transform.parent.transform.parent.GetComponent<DragonController>().SetupPhase2();
                    }
                }  
                //if the mouse clicks on the target during phase 2
                else if (isPhase1Done && !gameWon && rayHitInfo.collider.gameObject.CompareTag("Target"))
                {
                    
                    //instantiate the hit particle
                    GameObject particle = Instantiate(hitParticle, rayHitInfo.point, Quaternion.identity);
                    Destroy(particle, 5.0f); //destroy the particle object after 5 seconds

                    //set the dragon's animator
                    GameObject root = rayHitInfo.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;
                    dragonAnimator = root.transform.parent.gameObject.GetComponent<Animator>();

                    //Debug.Log("target: dragon is dead");
                    dragonAnimator.SetBool("isDead", true); //death animation
                    rayHitInfo.collider.gameObject.SetActive(false); //hide target

                    totalDragonsKilled++;
                }
                //also check if it collided with the dragon as a whole
                else if (isPhase1Done && !gameWon && rayHitInfo.collider.gameObject.CompareTag("Enemy"))
                {
                    //instantiate the hit particle
                    GameObject particle = Instantiate(hitParticle, rayHitInfo.point, Quaternion.identity);
                    Destroy(particle, 5.0f); //destroy the particle object after 5 seconds

                    //set the dragon's animator
                    GameObject root = rayHitInfo.collider.gameObject.transform.parent.gameObject;
                    dragonAnimator = root.transform.parent.gameObject.GetComponent<Animator>();

                    //Debug.Log("enemy: dragon is dead");
                    dragonAnimator.SetBool("isDead", true); //death animation
                    rayHitInfo.collider.gameObject.transform.parent.transform.Find("FinalTarget").gameObject.SetActive(false); //hide target

                    totalDragonsKilled++;                    
                }
                else
                {
                    //standard white particle instantiated at the raycast point position
                    GameObject particle = Instantiate(shootParticle, rayHitInfo.point, Quaternion.identity);
                    Destroy(particle, 5.0f); //destroy the particle object after 5 seconds
                }


                if(totalDragonsKilled >= (dragonsToSpawn+1))
                {
                    gameWon = true;
                }
            }
        }  
        
        //grow the player if the player is in phase 2 and is below a certain height
        if(isPhase1Done && this.transform.localScale.y < finalPlayerHeight)
        {
            this.transform.localScale += scaleChange;
        }

        if(Input.GetKeyDown(KeyCode.U)) //&& gameWon
        {
            //restart the scene
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        if(gameWon)
        {
            ShowReloadSceneInstructions();
        }
    }

    void StartPhase2()
    {
        //remove current targets
        GameObject.Find("Targets").SetActive(false);
        //disable collider for phase 1
        GameObject.Find("DragonColliderPhase1").SetActive(false);
        
        //grow player ( in update )

        //spawn a bunch of dragons across the scene
        SpawnDragons(dragonsToSpawn);
        
        //Debug.Log("phase 2 begins!!!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy") && !isPhase1Done)
        {
            //when the player goes into the dragon colliders, they get hurt

            playerController.lifecycle.Damage(20.0f);
        }
    }
    
    void HideInstructions()
    {
        //hide instructions

        Instructions.text = "";
    }

    void hitNumberUpdate()
    {
        if(!isPhase1Done)
        {
            hitnumberUI.text = numberOfTargetsHit + "/"+totalNumberOfTargets+" HIT";
        }
        else if(isPhase1Done)
        {
            hitnumberUI.text = "";
        }
    }


    void ShowReloadSceneInstructions()
    {
        //TODO REFERENCE A DIFFERENT INSTRUCTION THAT WOULD NOT BE AT THE TOP OF THE SCREEN, BUT LOWER
        Instructions.text = "Press U to Restart the Game";
    }


    void SpawnDragons(int amount)
    {
        //todo instantiate dragon game objects

        for(int i = 0; i < amount; i++)
        {
            GameObject newDragon = GameObject.Instantiate(DragonPrefab, RandomVector(0.0f, 100.0f), Quaternion.identity);
        }

    }

    Vector3 RandomVector(float min, float max)
    {
        Vector3 minPosition = new Vector3(min,min,min);

        Vector3 maxPosition = new Vector3(max,max,max);

        Vector3 v = new Vector3(Random.Range(minPosition.x, maxPosition.x), 0, Random.Range(minPosition.z, maxPosition.z));

        return v;
    }
}
