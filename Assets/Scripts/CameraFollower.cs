using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //check local player is exist and its not dead
        if (PlayerController.instance != null && !PlayerController.instance.dead)
        {
            //pass the info of player position to target position
            Vector3 targetPosition = PlayerController.instance.transform.position;

            //but z value will be as default of -10
            targetPosition.z = -10;

            //transform of camera will be where player position is
            transform.position = targetPosition;
        }
    }
}
