using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    private CarScript playerCarScript;
    private bool playerIsActive = false;
    [SerializeField]
    private Button accelerateButton;
    [SerializeField]
    private Button deaccelerateButton;
    [SerializeField]
    private Button doNothingButton;
    [SerializeField]
    private Button moveUpButton;
    [SerializeField]
    private Button moveDownButton;
    [SerializeField]
    private Text currentSpeed;
    private bool checkingButton = false;

    public bool PlayerIsActive
    {
        get { return playerIsActive; }
    }

    public void activatePlayer()
    {
        if(!playerIsActive)
        {
            currentSpeed.text = "Current Speed: " + playerCarScript.CurrentSpeed;
            playerIsActive = true;
            if (playerCarScript.canAccelerate())
            {
                activateButton(accelerateButton);
            } else
            {
                deactivateButton(accelerateButton);
            }
            if (playerCarScript.canDeaccelerate())
            {
                activateButton(deaccelerateButton);
            }
            else
            {
                deactivateButton(deaccelerateButton);
            }
            if (playerCarScript.canDoNothing())
            {
                activateButton(doNothingButton);
            }
            else
            {
                deactivateButton(doNothingButton);
            }
            if (playerCarScript.canMoveUp())
            {
                activateButton(moveUpButton);
            }
            else
            {
                deactivateButton(moveUpButton);
            }
            if (playerCarScript.canMoveDown())
            {
                activateButton(moveDownButton);
            }
            else
            {
                deactivateButton(moveDownButton);
            }
        }
    }

    private void playerHaveMadeNextAction()
    {
        deactivateButton(accelerateButton);
        deactivateButton(deaccelerateButton);
        deactivateButton(doNothingButton);
        deactivateButton(moveUpButton);
        deactivateButton(moveDownButton);
        currentSpeed.text = "Current Speed: " + playerCarScript.CurrentSpeed;
        playerIsActive = false;
    }

    public void accelerate()
    {
        if(!checkingButton && playerIsActive)
        {
            checkingButton = true;
            playerCarScript.accelerate();
            playerHaveMadeNextAction();
            checkingButton = false;
        }
    }
    public void deaccelerate()
    {
        if (!checkingButton && playerIsActive)
        {
            checkingButton = true;
            playerCarScript.deaccelerate();
            playerHaveMadeNextAction();
            checkingButton = false;
        }
    }
    public void doNothing()
    {
        if (!checkingButton && playerIsActive)
        {
            checkingButton = true;
            playerCarScript.doNothing();
            playerHaveMadeNextAction();
            checkingButton = false;
        }
    }
    public void moveUp()
    {
        if (!checkingButton && playerIsActive)
        {
            checkingButton = true;
            playerCarScript.moveUp();
            playerHaveMadeNextAction();
            checkingButton = false;
        }
    }
    public void moveDown()
    {
        if (!checkingButton && playerIsActive)
        {
            checkingButton = true;
            playerCarScript.moveDown();
            playerHaveMadeNextAction();
            checkingButton = false;
        }
    }

    private void deactivateButton(Button button)
    {
        button.GetComponent<Image>().enabled = false;
        button.GetComponent<Button>().enabled = false;
        button.transform.GetChild(0).GetComponent<Text>().enabled = false;
    }

    private void activateButton(Button button)
    {
        button.GetComponent<Image>().enabled = true;
        button.GetComponent<Button>().enabled = true;
        button.transform.GetChild(0).GetComponent<Text>().enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
