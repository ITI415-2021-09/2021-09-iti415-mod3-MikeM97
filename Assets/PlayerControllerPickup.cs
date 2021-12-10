using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerPickup : MonoBehaviour
{

    public Text countText;
    public Text winText;

    private int count;

    void Start()
    {
        count = 0;
        SetCountText();
        winText.text = "";
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Stars: " + count.ToString() + "/10";
        if (count >= 10)
        {
            winText.text = "Congratulations, you win!";
        }
    }
}