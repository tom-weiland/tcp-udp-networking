using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

public class PingServer : MonoBehaviour
{
    public static int serverMilliseconds;
    public static int serverSeconds;

    public static void CalculatePing()
    {
        int clientSeconds = DateTime.Now.Second;
        int clientMilliseconds = DateTime.Now.Millisecond;

        if (clientMilliseconds > serverMilliseconds)
        {
            if (clientSeconds > serverSeconds)
            {
                Debug.Log("ping is way to fucking high");
            }
            else
            {
                Debug.Log($"ping: {clientSeconds - serverSeconds}");
            }
        }
        else
        {
            Debug.Log($"ping: {clientMilliseconds - serverMilliseconds} tick: {SharedVariables.clientTick}");
        }
    }
}
