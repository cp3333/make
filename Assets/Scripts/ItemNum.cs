using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemNum : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    string originName;
    Vector3 originPos;
    Transform PanelObj;
    Transform PanelCurrentObj;

    // Use this for initialization
    void Start()
    {
        PanelObj = GameObject.FindGameObjectWithTag("PanelTag").transform;
        PanelCurrentObj = GameObject.FindGameObjectWithTag("PanelCurrentTag").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().pivot.Set(0, 0);
        transform.SetParent(PanelCurrentObj);
        transform.position = Input.mousePosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originName = transform.name.Substring(0,1);//str=str.Substring(0,i)
        originPos = transform.position;
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        if (PanelObj.childCount == 9)
        {
            GameObject Item = (GameObject)Instantiate(Resources.Load("Prefabs/" + originName), originPos, transform.rotation);
            Item.transform.SetParent(PanelObj);
        }
    }
}
