using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public GameObject shootParticle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                //out keyword means that we are returning our output result from the method call to that variable (it's a c# feature)


                //now we want to look at the raycast hit variable 

                //we can have our particle system be instantiated at the raycast point position
                GameObject.Instantiate(shootParticle, rayHitInfo.point, Quaternion.identity);
                Debug.Log("shot at "+ rayHitInfo.point);

                // but it can be any prefab in our assets!!! like platforms to navigate spaces

                // we could allow the player to destroy the things they created too, or give them a timeout that destroys them
                // clicking everywhere 
                // or having other input create ray casts?

                // my particles on the floor sorta looks like smoke


                //separate idea: if we want to know what we clicked
                // if the mouse clicks on an object with this tag:
                /*
                if (rayHitInfo.collider.gameObject.CompareTag("Pickup"))
                {
                    // we destroy the cube (add a bad sound effect to show we did a bad thing)
                    GameObject.Destroy(rayHitInfo.collider.gameObject);
                }
                else
                {
                    GameObject.Instantiate(prefab, rayHitInfo.point, Quaternion.identity);
                }
                */

            }

        }
    }
}
