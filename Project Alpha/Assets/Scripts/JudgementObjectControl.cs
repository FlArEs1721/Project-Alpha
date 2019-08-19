using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementObjectControl : MonoBehaviour
{
    [HideInInspector]
    public bool isStarted = false;    

    private SpriteRenderer spriteRenderer;

    private float temp = 0;

    private const float LifeTime = 0.7f;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, 0);
        if (isStarted)
        {
            if (temp < LifeTime)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.clear, temp / LifeTime);

                temp += Time.deltaTime;
            }
            else
            {
                //gameObject.SetActive(false);
            }
        }
    }

    public void Initialize()
    {
        isStarted = true;
    }
}
