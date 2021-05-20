using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int goal = 3;
    public float timer = 60.0f;
    public Dictionary<string, int> items;
    public Dictionary<string, GameObject> test;
    public string s;
    public int total;
    public int score;
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
    public Text timerText;
    public Text receipt;
    public Text transitionText;
    public Text budgetText;
    public Text levelText;
    public Text goalText;
    public GameObject pb;
    public GameObject jelly;
    public GameObject bread;
    public GameObject cereal;
    public GameObject milk;
    public GameObject pasta;
    public GameObject chicken;
    public GameObject eggs;
    public GameObject bacon;
    public int spawnInt;
    public float rotationSpeed;

    // Initializes item dictionary
    void Start()
    {
        rotationSpeed = 0.1f;
        items = new Dictionary<string, int>();
        test = new Dictionary<string, GameObject>();
        if(SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            timerText = GameObject.Find("Timer").GetComponent<Text>();
            receipt = GameObject.Find("Receipt").GetComponent<Text>();
            transitionText = GameObject.Find("Transition Text").GetComponent<Text>();
            levelText = GameObject.Find("Level Text").GetComponent<Text>();
            goalText = GameObject.Find("Goal").GetComponent<Text>();
        }
        if (SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            budgetText = GameObject.Find("Budget").GetComponent<Text>();
        }
    }

    // Starts timer count down & makes click start functionality for intro scene
    void Update()
    {
        timer -= Time.deltaTime;
        if (SceneManager.GetActiveScene().name == "Intro" && XRInputManager.IsButtonDown())
        {
            SceneManager.LoadScene("Level 1");
        }
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
        if(SceneManager.GetActiveScene().name == "1-2Transition" && inTransition1 == false)
        {
            setTransitionScene();
            inTransition1 = true;
        }
        if (SceneManager.GetActiveScene().name == "2-3Transition" && inTransition2 == false)
        {
            setTransitionScene();
            inTransition2 = true;
        }
        if(SceneManager.GetActiveScene().name == "1-2Transition" || SceneManager.GetActiveScene().name == "2-3Transition")
        {
            foreach (KeyValuePair<string, GameObject> entry in test)
            {
                entry.Value.transform.Rotate(new Vector3(0, rotationSpeed,0),Space.Self);
            }
        }
        if (SceneManager.GetActiveScene().name == "1-2Transition" && XRInputManager.IsButtonDown())
        {
            
            SceneManager.LoadScene("Level 2");
            foreach (KeyValuePair<string, GameObject> entry in test)
            {
                Destroy(entry.Value);
            }
            Destroy(this.gameObject);
        }
        if (SceneManager.GetActiveScene().name == "2-3Transition" && XRInputManager.IsButtonDown())
        {

            SceneManager.LoadScene("Level 3");
            foreach (KeyValuePair<string, GameObject> entry in test)
            {
                Destroy(entry.Value);
            }
            Destroy(this.gameObject);
        }

    }

    // Makes UI for game
    void OnGUI()
    {
        //Create UI timer in top left of screen for Level 1 & 2
        if (SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" || SceneManager.GetActiveScene().name == "Level 3")
        {
            timerText.text = "Time left: " + Mathf.Round(timer);
        }

        setLevel1();
        setLevel2();
        setLevel3();

        //***************************************************Level 1*********************************************************************
        //Once timer hits 0, checks how many meals were made and makes your receipt. Also gives you some transition text
        if (timer <= 0 && SceneManager.GetActiveScene().name == "Level 1")
        {
            Time.timeScale = 0.0f;

            checkMealsLevel2();

            Receipt();

            transitionText.text = "Great practice! \nNext round you must stay below a budget.\nClick to move to Level 2!";

            if (XRInputManager.IsButtonDown() && SceneManager.GetActiveScene().name == "Level 1")
            {
                DontDestroyOnLoad(this.gameObject);
                SceneManager.LoadScene("1-2Transition");
            }
        }
        //**************************************************Level 2***********************************************************************
        //Once timer hits 0, Checks how many meals were made and makes your receipt. Then gives you the proper end scene based on
        //or pushes you to level 3
        if (timer <= 0 && SceneManager.GetActiveScene().name == "Level 2")
        {
            Time.timeScale = 0.0f;

            checkMealsLevel2();

            Receipt();

            transitionText.text = "Click to see if you made it to level 3!";

            //Win
            if (score >= 3 && total < 35 && XRInputManager.IsButtonDown())
            {
                DontDestroyOnLoad(this.gameObject);
                SceneManager.LoadScene("2-3Transition");
            }
            //Over budget
            else if (XRInputManager.IsButtonDown() && total > 35)
            {
                SceneManager.LoadScene("Budget Loss");
            }
            //Not enough meals
            else if (XRInputManager.IsButtonDown() && score < 3)
            {
                SceneManager.LoadScene("Score Loss");
            }
            //Not enough meals and over budget
            else if (XRInputManager.IsButtonDown() && score < 3 && total > 35)
            {
                SceneManager.LoadScene("Both Loss");
            }
        }
        //Level 3
        if (timer <= 0 && SceneManager.GetActiveScene().name == "Level 3")
        {
            Time.timeScale = 0.0f;

            checkMealsLevel3();

            Receipt();

            transitionText.text = "Click to see if you passed level 3!";

            //Win
            if (score >= 4 && total < 40 && XRInputManager.IsButtonDown())
            {
                SceneManager.LoadScene("Win Scene");
            }
            //Over budget
            else if (XRInputManager.IsButtonDown() && total > 40)
            {
                SceneManager.LoadScene("Budget Loss");
            }
            //Not enough meals
            else if (XRInputManager.IsButtonDown() && score < 4)
            {
                SceneManager.LoadScene("Score Loss");
            }
            //Not enough meals and over budget
            else if (XRInputManager.IsButtonDown() && score < 4 && total > 40)
            {
                SceneManager.LoadScene("Both Loss");
            }
        }
    }

    public void setLevel1()
    {
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            levelText.text = "Level 1";
            goalText.text = "Goal: 3/4 recipes";
        }
    }
    //Resets timer for level 2 and makes budget UI text
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
    public void setTransitionScene()
    {
        spawnInt = 0;
        foreach(KeyValuePair<string,GameObject> entry in test)
        {
            entry.Value.gameObject.transform.position = new Vector3(spawnInt, 0, 0);
            Renderer[] t = entry.Value.GetComponentsInChildren<Renderer>();
            for(int i = 0; i < t.Length; i++)
            {
                t[i].enabled = true;
            }
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
                s += entry.Key + " $" + entry.Value + "\n";
                total += entry.Value;
            }
            //Makes total price
            s += "\ntotal price: $" + total;
            inFE = true;
        }
        //Creates UI for receipt
        if (items.Count > 0)
        {
            receipt.text = "Receipt:\n\n" + s + "\nScore: " + score;
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

        if (items.ContainsKey("Tomatos") && items.ContainsKey("Lettuce") && items.ContainsKey("Carrots") && inSalad == false)
        {
            score += 1;
            inSalad = true;
        }

        if (items.ContainsKey("Bread") && items.ContainsKey("Bananas") && inBB == false)
        {
            score += 1;
            inBB = true;
        }
    }
}
