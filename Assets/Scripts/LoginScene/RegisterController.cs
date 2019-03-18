using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    Vector3 origin;
    Vector3 target;
    public Button hide;
    public bool show = false;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        target = new Vector3(0, 0);
        hide.onClick.AddListener(() => { show = false; });
    }

    // Update is called once per frame
    void Update()
    {
        if (show == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 150 * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, origin, 150 * Time.deltaTime);
        }
    }
}
