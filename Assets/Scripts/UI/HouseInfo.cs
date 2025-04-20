using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseInfo : MonoBehaviour
{

    public GameObject Object;
    public Transform transform;
    public Vector2 offets;
    public TMP_Text titletext;
    public TMP_Text descriptiontext;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Object.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            int layerObject = 8;
            Vector2 ray = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(ray, ray);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("House"))
                {
                    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    transform.position = pos + offets;
                    titletext.SetText(hit.collider.gameObject.GetComponent<TestObject>().title);
                    descriptiontext.SetText(hit.collider.gameObject.GetComponent<TestObject>().description);
                    Object.SetActive(true);
                }
            }
            else Object.SetActive(false);
        }
            
    }
}
