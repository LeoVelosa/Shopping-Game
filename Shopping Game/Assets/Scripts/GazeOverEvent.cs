using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GazeOverEvent : MonoBehaviour
{
    /// <summary>
    /// Margin of error for a valid gaze. The number of degrees off from a direct
    /// gaze that will still be counted as looking at the object.
    /// </summary>
    /// <remarks>This should be larger for larger objects.</remarks>
    [Range(0, 360)]
    public float maximumAngleForEvent = 15f;

    //Hover variables
    public UnityEvent OnHoverBegin;
    public UnityEvent OnHover;
    public UnityEvent OnHoverEnd;
    public UnityEvent OnButtonPressedDuringHover;

    //creates level manager variable
    public LevelManager l;

    //creates price variable to hold the item hovered's price
    public int price;

    //In game text variables
    public Text priceText;
    public Text pauseText;

    //holds pause status
    public bool paused = false;

    //Audio variables
    public AudioSource pickup;
    public AudioSource music;

    /// <summary>
    /// A boolean that tracks if the object is currently hovered over. Used to
    /// ensure OnHoverBegin and OnHoverEnd are only fired once per gaze start/end.
    /// </summary>
    private bool isHovering = false;

    // Initializes many variables
    void Start()
    {
        //Initializes level manager
        l = GameObject.Find("Level Manager").GetComponent<LevelManager>();

        //Checks if in a level and initializes proper text and audio
        if(SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            priceText = GameObject.Find("Price").GetComponent<Text>();
            pauseText = GameObject.Find("Pause Text").GetComponent<Text>();
            pickup = GameObject.Find("Pickup SFX").GetComponent<AudioSource>();
            music = GameObject.Find("Music").GetComponent<AudioSource>();
        }
    }

    //Checks if you are hovering over an object that contains this script
    void Update()
    {
        var cameraForward = Camera.main.transform.forward;
        var vectorToObject = transform.position - Camera.main.transform.position;

        // Check if the angle between the camera and object is within the hover range
        var angleFromCameraToObject = Vector3.Angle(cameraForward, vectorToObject);
        if (angleFromCameraToObject <= maximumAngleForEvent)
        {
            Hovering();

            if (XRInputManager.IsButtonDown())
            {
                ButtonPressed();
            }
        }
        else
        {
            NotHovering();
        }
    }

    private void Hovering()
    {
        if (isHovering)
        {
            //OnHover.Invoke();
            DisplayItemInfo();
        }
        else
        {
            //OnHoverBegin.Invoke();
            isHovering = true;
        }
    }

    private void NotHovering()
    {
        if (isHovering)
        {
            //checks if in a level
            if(SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
            {
                //Sets price text to an empty string so that when you are not hovering over an object the text goes away
                priceText.text = "";
            }
            OnHoverEnd.Invoke();
            isHovering = false;
        }
    }

    private void ButtonPressed()
    {
        OnButtonPressedDuringHover.Invoke();
        //Pauses game
        if(this.gameObject.name == "Pause Button" && paused == false)
        {
            //pauses background music
            music.Pause();

            //stops timer
            Time.timeScale = 0.0f;

            //sets on screen text to tell the player the game is paused and how to unpause
            pauseText.text = "Paused\nPress pause button to unpause";

            //sets paused to true
            paused = true;
        }

        //Unpauses game
        else if(this.gameObject.name == "Pause Button" && paused == true)
        {
            //unpauses background music
            music.UnPause();

            //starts timer again
            Time.timeScale = 1.0f;

            //removes pause text
            pauseText.text = "";

            //sets paused to false;
            paused = false;
        }

        //checks if the finished early button is pressed
        else if(this.gameObject.name == "Finished Early Button")
        {
            //sets timer to 0 to trigger end of level
            l.timer = 0.0f;
        }

        //If not pause or finished early button (I.E all the foods)
        else
        {
            //Adds the item clicked to a dictionary to keep track of price and ingredient.
            l.items.Add(this.gameObject.name, this.price);

            //Adds gameObject to a dictionary to be used in transition scene
            l.gameObjectHolder.Add(this.gameObject.name, this.gameObject);

            //Makes food clicked not get destroyed when moving to next scene so that it can be used in transition scene
            DontDestroyOnLoad(this.gameObject);

            //Creates list of the foods children
            Renderer[] foodChildren = this.gameObject.GetComponentsInChildren<Renderer>();

            //Iterates through the foods children list
            for(int i = 0; i < foodChildren.Length; i++)
            {
                //Sets the foods mesh renderer to false so that the food disappears when clicked
                foodChildren[i].enabled = false;
            }

            //Plays pickup sound effect when food is clicked
            pickup.Play();
        }
    }

    //Sets the price text to the name of the hovered object and its price
    public void DisplayItemInfo()
    {
        //Checks if item hovered is pause button
        if(this.gameObject.name == "Pause Button")
        {
            //Checks if game is currently paused
            if (paused)
            {
                //sets pause button hover text so the player knows to click it again to begin playing again
                priceText.text = "Click here to play";
            }
            //if not currently paused
            else
            {
                //sets pause button hover text so the player knows to click it to pause
                priceText.text = "Click here to pause the game";
            }
        }
        //If item hovered is finished early button
        else if(this.gameObject.name == "Finished Early Button")
        {
            //sets finished early buttons hover text
            priceText.text = "Click here if you finish before time is up.";
        }
        //Makes sure that hover text for food only works in levels
        else if(SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            //Sets foods hover text to the foods name and its price
            priceText.text = this.gameObject.name + "\nPrice: $" + price;
        }
    }
}