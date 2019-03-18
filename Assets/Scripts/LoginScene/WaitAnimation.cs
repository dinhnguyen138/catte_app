using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAnimation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(-(Vector3.forward * 200 * Time.deltaTime), Space.Self);
    }
}
