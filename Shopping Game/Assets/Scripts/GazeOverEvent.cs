using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// MonoBehavior for an interactable objects. Triggers different <c>UnityEvent</c>s
/// (sets of functions) when the user is gazing at the object. The margin of error
/// for gazing (in degrees) can be adjusted to allow for more generous, comfortable
/// control.
/// </summary>
public class GazeOverEvent : MonoBehaviour
{
    /// <summary>
    /// Margin of error for a valid gaze. The number of degrees off from a direct
    /// gaze that will still be counted as looking at the object.
    /// </summary>
    /// <remarks>This should be larger for larger objects.</remarks>
    [Range(0, 360)]
    public float maximumAngleForEvent = 15f;

    public UnityEvent OnHoverBegin;
    public UnityEvent OnHover;
    public UnityEvent OnHoverEnd;
    public UnityEvent OnButtonPressedDuringHover;
    public LevelManager l;
    public int price;
    public int spent;
    public Text priceText;
    public Text pauseText;
    public bool paused = false;
    public AudioSource pickup;
    public AudioSource music;

    /// <summary>
    /// A boolean that tracks if the object is currently hovered over. Used to
    /// ensure OnHoverBegin and OnHoverEnd are only fired once per gaze start/end.
    /// </summary>
    private bool isHovering = false;

    // Initializes levelmanager script and the UI Price text for future use
    void Start()
    {
        l = GameObject.Find("Level Manager").GetComponent<LevelManager>();
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
            music.Pause();
            Time.timeScale = 0.0f;
            pauseText.text = "Paused\nPress pause button to unpause";
            paused = true;
        }
        //Unpauses
        else if(this.gameObject.name == "Pause Button" && paused == true)
        {
            music.UnPause();
            Time.timeScale = 1.0f;
            pauseText.text = "";
            paused = false;
        }
        else if(this.gameObject.name == "Finished Early Button")
        {
            l.timer = 0.0f;
        }
        else
        {
            //Adds the item clicked to a dictionary to keep track of price and ingredient.
            l.items.Add(this.gameObject.name, this.price);
            l.test.Add(this.gameObject.name, this.gameObject);

            DontDestroyOnLoad(this.gameObject);
            
            Renderer[] a = this.gameObject.GetComponentsInChildren<Renderer>();
            for(int i = 0; i < a.Length; i++)
            {
                a[i].enabled = false;
            }

            Debug.Log(pickup);
            pickup.Play();
            Debug.Log(this);
        }
    }

    //Sets the price text to the name of the hovered object and its price
    public void DisplayItemInfo()
    {
        if(this.gameObject.name == "Pause Button")
        {
            if (paused)
            {
                priceText.text = "Click here to play";
            }
            else
            {
                priceText.text = "Click here to pause the game";
            }
        }
        else if(this.gameObject.name == "Finished Early Button")
        {
            priceText.text = "Click here if you finish before time is up.";
        }
        else if(SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            priceText.text = this.gameObject.name + "\nPrice: $" + price;
        }
    }
}