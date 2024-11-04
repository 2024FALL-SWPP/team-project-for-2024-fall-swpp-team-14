using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneLaserController : MonoBehaviour
{
    private float droneLaserSpeed = 10.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.forward * Time.deltaTime * droneLaserSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
