using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sc_DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Sc_GameAdditionController GameAdditionController;


    public void OnPointerEnter(PointerEventData eventData) {

        if (eventData.pointerDrag == null)
            return;

        Sc_Draggable d = eventData.pointerDrag.GetComponent<Sc_Draggable>();
        if (d != null)
        {
            //d.placeHolderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (eventData.pointerDrag == null)
            return;

        Sc_Draggable d = eventData.pointerDrag.GetComponent<Sc_Draggable>();
        if (d != null && d.placeHolderParent == this.transform)
        {
            d.placeHolderParent = d.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData) {
       // Debug.Log(eventData.pointerDrag.name + " dropped on " + gameObject.name);

        Sc_Draggable d = eventData.pointerDrag.GetComponent<Sc_Draggable>();
        if (gameObject.tag.Equals("AnswerDropContainer")){
            for(var i=0; i<transform.childCount;i++) {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        if (d != null) {
            d.parentToReturnTo = this.transform;
        }

        if (gameObject.tag.Equals("AnswerDropContainer")) {
            GameAdditionController.OnReadyAnswerDrop(eventData.pointerDrag);            
        }
    }
}
