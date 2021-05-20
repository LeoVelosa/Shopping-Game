using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MoveToClickPoint : MonoBehaviour
{
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Google.XR.Cardboard.Api.IsTriggerPressed || Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50))
            {
                if(hit.transform.gameObject.name == "XboxController")
                {
                    SceneManager.LoadScene("Level2");
                }
                else
                {
                    this.transform.position = hit.point;
                }

            }
        }
    }
}
