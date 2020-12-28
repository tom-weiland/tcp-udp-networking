using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiCheat : MonoBehaviour
{

    public static void DoCheck(Vector3 serverPositon)
    {
       //if (PlayerController.controller.transform.position.x != serverPositon.x)
       //{
       //     PlayerController.controller.enabled = false;
       //     PlayerController.playerTransform.transform.position = new Vector3(serverPositon.x, PlayerController.playerTransform.position.y, PlayerController.playerTransform.position.z);
       //     PlayerController.controller.enabled = true;
       //     Debug.Log("uh oh");
       //}
       //if (PlayerController.controller.transform.position.z != serverPositon.z)
       //{
       //     PlayerController.controller.enabled = false;
       //     PlayerController.playerTransform.transform.position = new Vector3(PlayerController.playerTransform.position.x, PlayerController.playerTransform.position.y, serverPositon.z);
       //     PlayerController.controller.enabled = true;
       //     Debug.Log("uh oh");
       // }
      
       //else
       //{
       //     return;
       //}
       // //this.transform.position = Vector3.Lerp(fromPos, toPos, (Time.time - lastTime) / (1.0f / Constants.TICKS_PER_SEC));
    }
}
