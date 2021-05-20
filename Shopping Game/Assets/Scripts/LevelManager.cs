using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //Timer
    public float timer = 60.0f;

    //Used to hold the item names and prices the player buys
    public Dictionary<string, int> items;

    //Used to hold gameObjects the player buys so that they get displayed in the transition scene
    public Dictionary<string, GameObject> gameObjectHolder;

    public string stringHolder;

    //Total amount of money the player paid at the end of the level
    public int totalPaid;

    //Used to hold score of the player
    public int score;

    //Booleans to stop infinite loops
    public bool inPB = false;
    public bool inCereal = false;
    public bool inPasta = false;
    public bool inRice = false;
    public bool inSalad = false;
    public bool inFE = false;
    public bool inEB = false;
    public bool inBB = false;
    public bool inTransition1 = false;
    public bool inTransition2 = false;

    //In game text variables
    public Text timerText;
    public Text receipt;
    public Text transitionText;
    public Text budgetText;
    public Text levelText;
    public Text goalText;

    //Integer used to spawn gameObjects in transition scenes
    public int spawnInt;

    //Float used to set speed of gameObject rotation in transition scenes
    public float rotationSpeed;

    // Initializes many variables
    void Start()
    {
        //Initializes rotation speed for transition scene
        rotationSpeed = 0.1f;

        //Initializes both dictionaries
        items = new Dictionary<string, int>();
        gameObjectHolder = new Dictionary<string, GameObject>();

        //Initializes in game text
        if(SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            timerText = GameObject.Find("Timer").GetComponent<Text>();
            receipt = GameObject.Find("Receipt").GetComponent<Text>();
            transitionText = GameObject.Find("Transition Text").GetComponent<Text>();
            levelText = GameObject.Find("Level Text").GetComponent<Text>();
            goalText = GameObject.Find("Goal").GetComponent<Text>();
        }

        //Initializes budget text for level 2 & 3
        if (SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            budgetText = GameObject.Find("Budget").GetComponent<Text>();
        }
    }

    // Used for any functions that need to be constantly checked for
    void Update()
    {
        //Starts timer countdown
        timer -= Time.deltaTime;

        //Sets click start for intro scene
        if (SceneManager.GetActiveScene().name == "Intro" && XRInputManager.IsButtonDown())
        {
            SceneManager.LoadScene("Level 1");
        }
        //Sets click restart for end scenes
        if (SceneManager.GetActiveScene().name == "Win Scene" && XRInputManager.IsButtonDown())
        {
            SceneManager.LoadScene("Level 1");
        }
        if (SceneManager.GetActiveScene().name == "Score Loss" && XRInputManager.IsButtonDown())
        {
            SceneManager.LoadScene("Level 1");
        }
        if (SceneManager.GetActiveScene().name == "Budget Loss" && XRInputManager.IsButtonDown())
        {
            SceneManager.LoadScene("Level 1");
        }
        if (SceneManager.GetActiveScene().name == "Both Loss" && XRInputManager.IsButtonDown())
        {
            SceneManager.LoadScene("Level 1");
        }

        //Checks if in transition scene from level 1 to 2
        if(SceneManager.GetActiveScene().name == "1-2Transition" && inTransition1 == false)
        {
            //spawns items the player bought
            setTransitionScene();

            //sets boolean to true so this if statement only gets called once
            inTransition1 = true;
        }

        //Checks if in transition scene from level 2 to 3
        if (SceneManager.GetActiveScene().name == "2-3Transition" && inTransition2 == false)
        {
            //spawns items the player bought
            setTransitionScene();

            //sets boolean to true so this if statement only gets called once
            inTransition2 = true;
        }

        //Checks if in either transition scenes to set the objects rotation and constantly update it
        if(SceneManager.GetActiveScene().name == "1-2Transition" || SceneManager.GetActiveScene().name == "2-3Transition")
        {
            //Iterates through items the user bought and sets their rotation
            foreach (KeyValuePair<string, GameObject> entry in gameObjectHolder)
            {
                entry.Value.transform.Rotate(new Vector3(0, rotationSpeed, 0), Space.Self);
            }
        }

        //Destroys all items in "DontDestroyOnLoad" Scene to set up level 2
        if (SceneManager.GetActiveScene().name == "1-2Transition" && XRInputManager.IsButtonDown())
        {
            //Loads level 2
            SceneManager.LoadScene("Level 2");

            //Iterates through items in the transition scene and destroys them
            foreach (KeyValuePair<string, GameObject> entry in gameObjectHolder)
            {
                Destroy(entry.Value);
            }

            //Destroys the level manager in the "DontDestroyOnLoad" scene
            Destroy(this.gameObject);
        }
        //Destroys all items in "DontDestroyOnLoad" Scene to set up level 3
        if (SceneManager.GetActiveScene().name == "2-3Transition" && XRInputManager.IsButtonDown())
        {
            //Loads level 3
            SceneManager.LoadScene("Level 3");

            //Iterates through items in the transition scene and destroys them
            foreach (KeyValuePair<string, GameObject> entry in gameObjectHolder)
            {
                Destroy(entry.Value);
            }
            //Destroys the level manager in the "DontDestroyOnLoad" scene
            Destroy(this.gameObject);
        }

        //Create UI timer in top left of screen for Level 1 & 2
        if (SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            timerText.text = "Time left: " + Mathf.Round(timer);
        }

        //Functions check if in the designated level and sets it
        setLevel1();
        setLevel2();
        setLevel3();

        //***************************************************Level 1*********************************************************************
        //Once timer hits 0, checks how many meals were made and makes your receipt. Also gives you some transition text
        if (timer <= 0 && SceneManager.GetActiveScene().name == "Level 1")
        {
            //Stops timer
            Time.timeScale = 0.0f;

            //Calls meal check function
            checkMealsLevel2();

            //Creates on screen receipt
            Receipt();

            //Sets transition text
            transitionText.text = "Great practice! \nNext round you must stay below a budget.\nClick to move to Level 2!";

            //Checks if button is clicked then sends player to transition scene
            if (XRInputManager.IsButtonDown())
            {
                /* 
                 * Keeps level manager from level 1 and moves it to transition scene because it holds
                 * the dictionary that contains all the items the player bought in level 1.
                 */
                DontDestroyOnLoad(this.gameObject);

                //Loads transition scene
                SceneManager.LoadScene("1-2Transition");
            }
        }

        //**************************************************Level 2***********************************************************************
        /*
         * Once timer hits 0, Checks how many meals were made and makes your receipt. Then gives you the proper end scene
         * based on performance or pushes you to level 3 if you passed.
         */
        if (timer <= 0 && SceneManager.GetActiveScene().name == "Level 2")
        {
            //Stops timer
            Time.timeScale = 0.0f;

            //Calls meal check function
            checkMealsLevel2();

            //Creates on screen receipt
            Receipt();

            //Sets transition text
            transitionText.text = "Click to see if you made it to level 3!";

            //The next couple if statements check for win/loss conditions
            //Win - player moves on to level 3
            if (score >= 3 && totalPaid < 35 && XRInputManager.IsButtonDown())
            {
                DontDestroyOnLoad(this.gameObject);
                SceneManager.LoadScene("2-3Transition");
            }
            //Over budget
            else if (XRInputManager.IsButtonDown() && totalPaid > 35)
            {
                SceneManager.LoadScene("Budget Loss");
            }
            //Not enough meals
            else if (XRInputManager.IsButtonDown() && score < 3)
            {
                SceneManager.LoadScene("Score Loss");
            }
            //Not enough meals and over budget
            else if (XRInputManager.IsButtonDown() && score < 3 && totalPaid > 35)
            {
                SceneManager.LoadScene("Both Loss");
            }
        }
        //**************************************************Level 3***********************************************************************
        /*
         * Once timer hits 0, Checks how many meals were made and makes your receipt.
         * Then gives you the proper end scene based on performance.
         */
        if (timer <= 0 && SceneManager.GetActiveScene().name == "Level 3")
        {
            //Stops timer
            Time.timeScale = 0.0f;

            //Calls meal check function
            checkMealsLevel3();

            //Creates on screen receipt
            Receipt();

            //Sets transition text
            transitionText.text = "Click to see if you passed level 3!";

            //The next couple if statements check for win/loss conditions
            //Win
            if (score >= 4 && totalPaid < 40 && XRInputManager.IsButtonDown())
            {
                SceneManager.LoadScene("Win Scene");
            }
            //Over budget
            else if (XRInputManager.IsButtonDown() && totalPaid > 40)
            {
                SceneManager.LoadScene("Budget Loss");
            }
            //Not enough meals
            else if (XRInputManager.IsButtonDown() && score < 4)
            {
                SceneManager.LoadScene("Score Loss");
            }
            //Not enough meals and over budget
            else if (XRInputManager.IsButtonDown() && score < 4 && totalPaid > 40)
            {
                SceneManager.LoadScene("Both Loss");
            }
        }
    }

    //Sets level text and goal text
    public void setLevel1()
    {
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            levelText.text = "Level 1";
            goalText.text = "Goal: practice";
        }
    }

    //Resets timer for level 2 and sets all the necessary text for level 2
    public void setLevel2()
    {
        if (SceneManager.GetActiveScene().name == "Level 2")
        {
            levelText.text = "Level 2";
            budgetText.text = "Budget: $35";
            goalText.text = "Goal: 3/4 recipes";
            Time.timeScale = 1.0f;
        }
    }

    //Resets timer for level 2 and sets all the necessary text for level 3
    public void setLevel3()
    {
        if (SceneManager.GetActiveScene().name == "Level 3")
        {
            levelText.text = "Level 3";
            budgetText.text = "Budget: $40";
            goalText.text = "Goal: 4/6 recipes";
            Time.timeScale = 1.0f;
        }
    }

    //Sets transition scene
    public void setTransitionScene()
    {
        //sets the spawn variable to 0
        spawnInt = 0;

        //iterates through items the player bought
        foreach(KeyValuePair<string,GameObject> entry in gameObjectHolder)
        {
            //sets the items x position to the spawn int and a y&z of 0
            entry.Value.gameObject.transform.position = new Vector3(spawnInt, 0, 0);

            //Grabs all the items children mesh renderers
            Renderer[] itemsChildren = entry.Value.GetComponentsInChildren<Renderer>();

            //iterates through the items children mesh renderers
            for(int i = 0; i < itemsChildren.Length; i++)
            {
                //Sets the items childrens mesh renderers to true since they were set to false in the previous scene
                itemsChildren[i].enabled = true;
            }

            //Adds 2 to the spawn integer so the items are spaced out
            spawnInt += 2;
        }
    }
    //Creates string to display on receipt and makes UI
    public void Receipt()
    {
        //Checks if it has ran through the foreach loop before
        if (inFE == false)
        {
            //Makes string with name of item and its price
            foreach (KeyValuePair<string, int> entry in items)
            {
                stringHolder += entry.Key + " $" + entry.Value + "\n";
                totalPaid += entry.Value;
            }
            //Makes total price
            stringHolder += "\ntotal price: $" + totalPaid;
            inFE = true;
        }
        //Creates UI for receipt
        if (items.Count > 0)
        {
            receipt.text = "Receipt:\n\n" + stringHolder + "\nScore: " + score;
        }
    }

    //Checks if all the ingredients for each meal were bought
    public void checkMealsLevel2()
    {
        //PB&J
        if (items.ContainsKey("Peanut Butter") && items.ContainsKey("Jelly") && items.ContainsKey("Bread") && inPB == false)
        {
            score += 1;
            inPB = true;
        }

        //Cereal
        if (items.ContainsKey("Milk") && items.ContainsKey("Cereal") && inCereal == false)
        {
            score += 1;
            inCereal = true;
        }

        //Pasta & Chicken
        if (items.ContainsKey("Pasta Box") && items.ContainsKey("Chicken") && inPasta == false)
        {
            score += 1;
            inPasta = true;
        }

        //Eggs & Bacon
        if (items.ContainsKey("Eggs") && items.ContainsKey("Bacon") && inEB == false)
        {
            score += 1;
            inEB = true;
        }
    }

    //Checks if all the ingredients for each meal were bought
    public void checkMealsLevel3()
    {
        //PB&J
        if (items.ContainsKey("Peanut Butter") && items.ContainsKey("Jelly") && items.ContainsKey("Bread") && inPB == false)
        {
            score += 1;
            inPB = true;
        }

        //Cereal
        if (items.ContainsKey("Milk") && items.ContainsKey("Cereal") && inCereal == false)
        {
            score += 1;
            inCereal = true;
        }

        //Pasta & Chicken
        if (items.ContainsKey("Rice bag") && items.ContainsKey("Chicken") && inRice == false)
        {
            score += 1;
            inRice = true;
        }

        //Eggs & Bacon
        if (items.ContainsKey("Eggs") && items.ContainsKey("Bacon") && inEB == false)
        {
            score += 1;
            inEB = true;
        }

        //Salad
        if (items.ContainsKey("Tomatos") && items.ContainsKey("Lettuce") && items.ContainsKey("Carrots") && inSalad == false)
        {
            score += 1;
            inSalad = true;
        }

        //Banana bread
        if (items.ContainsKey("Bread") && items.ContainsKey("Bananas") && inBB == false)
        {
            score += 1;
            inBB = true;
        }
    }
}
