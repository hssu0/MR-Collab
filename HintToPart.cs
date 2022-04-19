using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HintToPart : MonoBehaviour
{
    private PartToHints partToHintsScript;

    private PhotonView pv;

    public bool hasPart = false;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Hint에 퍼즐 조각이 부딪힌 경우
    void OnTriggerEnter(Collider other)
    {
        //부딪힌 퍼즐 조각의 PartToHints 스크립 가져오기
        partToHintsScript = other.GetComponent<PartToHints>(); 

        if (!hasPart) //힌트에 퍼즐 조각이 없는 상태
        {
            //해당 퍼즐 조각을 힌트 위에 위치 고정
            partToHintsScript.SetPosition(transform); 

            //상대 작업자에게 해당 힌트 자리에 퍼즐이 고정되었음을 동기화(hasPart 상태가 true)
            pv = PhotonView.Get(this);
            pv.RPC("HintHasPart", RpcTarget.All); 
        }
    }

    public void ResetHint()
    {
        //Reset 버튼을 눌렀을 때 모든 힌트는 퍼즐 조각이 없는 상태로
        pv = PhotonView.Get(this);
        pv.RPC("ResetHasPart", RpcTarget.All);
    }

    [PunRPC]
    void HintHasPart()
    {
        hasPart = true;
    }

    [PunRPC]
    void ResetHasPart()
    {
        hasPart = false;
    }
}