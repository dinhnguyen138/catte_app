using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUser : MonoBehaviour
{
    public Transform activeIndicator;
    public Transform countDown;
    private UserStatus userStatus;
    [SerializeField] private float percentage;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        userStatus = GetComponent<UserStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if(userStatus.isActive == true)
        {
            activeIndicator.gameObject.SetActive(true);
            countDown.gameObject.SetActive(true);
            if (percentage < 100)
            {
                percentage += speed * Time.deltaTime;
                countDown.GetComponent<Image>().fillAmount = percentage / 100;
            }
            else
            {
                userStatus.isActive = false;
            }
        }
        else
        {
            activeIndicator.gameObject.SetActive(false);
            countDown.gameObject.SetActive(false);
            percentage = 0;
        }
    }
}
