using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f);
    [SerializeField] float period = 2f;

    [Range(0,1)] [SerializeField] float movementFactor;//0 to 1 for range of movement

    Vector3 startingPos;//stored for absolute movement


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if(period <= Mathf.Epsilon) { return; }

        //set movement factor
        float cycles = Time.time / (period*2); //grows continually from 0

        const float tau = Mathf.PI * 2; //about 6.28
        float rawSineWave = Mathf.Sin(cycles * tau); //value between -1 and 1

        movementFactor = rawSineWave / 2f + 0.5f; //becomes value between 0 and 1

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
