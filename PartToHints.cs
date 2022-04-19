using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PartToHints : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public bool isPlaced = false;

    private Rigidbody puzzleRigidbody;

    private PhotonView pv;

    private HintToPart hintToPartScript;

    private GameObject hint;

    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();

        var trans = transform;
        originalPosition = trans.localPosition;
        originalRotation = trans.localRotation;

        puzzleRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOriginalPos()
    {
        //Reset Button을 눌렀을 때 퍼즐 조각들이 처음 위치로 돌아감
        pv = PhotonView.Get(this);
        pv.RPC("OriginalPos", RpcTarget.All, "Reset Position");
    }

    //힌트에 고정된 퍼즐 조각을 한번 더 터치했을 때 떼어지도록 함
    public void SetIsPlaced()
    {
        if (isPlaced) //힌트에 고정된 퍼즐 조각인지 확인
        {
            //고정된 퍼즐 조각이 아닌 상태로(isPlaced = false)
            pv = PhotonView.Get(this);
            pv.RPC("PartisnotPlaced", RpcTarget.All);

            //고정시킨 Rigidbody Constraints를 모두 해제하고 힌트로부터 0.01f 만큼 떼어냄
            puzzleRigidbody.constraints = RigidbodyConstraints.None;
            transform.position = new Vector3(transform.position.x, transform.position.y, 
                transform.position.z - 0.01f);

            //해당 힌트도 퍼즐 조각을 갖고 있지 않은 상태(hasPart = false)로 바꿈
            hintToPartScript = hint.GetComponent<HintToPart>();
            hintToPartScript.hasPart = false;
        }
    }

    //힌트에 퍼즐 조각 고정
    //HintToPart에서 함수 호출
    //HintToPart에서 해당 퍼즐 조각이 부딪힌 힌트의 위치, 회전 값을 받아옴
    public void SetPosition(Transform trans) 
    {
        if (!isPlaced) //힌트에 고정되지 않은 퍼즐 조각인지 확인
        {
            //HintToPart에서 받아온 부딪힌 힌트의 위치, 회전 값으로 퍼즐 조각의 위치, 회전 값을 설정
            transform.position = trans.position;
            transform.rotation = trans.rotation;

            //해당 퍼즐의 Rigidbody 상태를 Sleep으로 바꾸고(움직이지 않는 상태인 것 같음),
            // Rigidbody Constraints를 모두 이 위치, 회전 값으로 고정
            puzzleRigidbody.Sleep();
            puzzleRigidbody.constraints = RigidbodyConstraints.FreezeAll;

            //모든 작업자들의 이 퍼즐 조각을 isPlaced = true 상태로 바꿈
            pv = PhotonView.Get(this);
            pv.RPC("PartisPlaced", RpcTarget.All);

            //고정 후 이 퍼즐 조각을 힌트에서 떼어낼 때, 고정되어 있는 힌트를 찾기 위해 저장해둠
            hint = trans.gameObject;
        }
    }

    [PunRPC]
    void OriginalPos(string a)
    {
        Debug.Log(string.Format(a));

        // 원래 위치로 돌아감
        gameObject.transform.localPosition = originalPosition;
        gameObject.transform.localRotation = originalRotation;

        // 모든 퍼즐 조각이 힌트에 위치해 있지 않은 상태로 설정
        pv = PhotonView.Get(this);
        pv.RPC("PartisnotPlaced", RpcTarget.All);

        // 모든 퍼즐 조각의 Rigidbody Constraints를 해제
        puzzleRigidbody.constraints = RigidbodyConstraints.None;

        puzzleRigidbody.Sleep();
        puzzleRigidbody.WakeUp();

    }

    [PunRPC]
    void PartisPlaced()
    {
        isPlaced = true;
    }

    [PunRPC]
    void PartisnotPlaced()
    {
        isPlaced = false;
    }

}
