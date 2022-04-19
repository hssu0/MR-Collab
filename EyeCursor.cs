using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using MRTK.Tutorials.MultiUserCapabilities;

public class EyeCursor : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private RaycastHit hit;

    private LineRenderer line;
    private MeshRenderer mesh;

    private PhotonView view;

    public bool state;
    public int userNum;

    public Vector3 startPos = Vector3.zero;

    private string myName;
    private int myNum;

    private OnOffRay onoffRayScript;// OnOffRay 스크립트

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;

        view = PhotonView.Get(this);

        onoffRayScript = GameObject.Find("OnOffButton").GetComponent<OnOffRay>();
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(mesh.enabled); //mesh.enabled 상태 (hit 상태)
            stream.SendNext(myNum); // User 번호

        }
        else
        {
            state = (bool)stream.ReceiveNext();
            userNum = (int)stream.ReceiveNext();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        myName = gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.name; //User 이름
        myNum = int.Parse(myName[4].ToString()); //User 번호 추출 (몇 번째 User인지)

        if (view.IsMine) // PhotonView의 IsMine = true 인 객체
        {
            GazeRay();

            line.enabled = false; // 자신의 Ray invisible
        }
        else // PhotonView의 IsMine = false 인 객체
        {
            if (onoffRayScript.onoffstate == true) //on
            {
                line.enabled = state;
                mesh.enabled = state;

                //이름별로 색 설정
                if (userNum == 1) //User1
                {
                    line.material.color = Color.yellow;
                }
                else if (userNum == 2) //User2
                {
                    line.material.color = Color.red;
                }
                else //User3 ...
                {
                    line.material.color = Color.blue;
                }

                //Ray의 시작은 PhotonUser(EyeCursor의 부모 객체)
                startPos = gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.position;

                line.SetPosition(1, startPos);
                line.SetPosition(0, gameObject.transform.position);
            }
            else //off
            {
                mesh.enabled = false; //cursor
                line.enabled = false; //ray
            }
        }

    }

    void GazeRay()
    {
        var eyeGazeProvider = Microsoft.MixedReality.Toolkit.CoreServices.InputSystem?.EyeGazeProvider;
        if (eyeGazeProvider != null)
        {
            if (UnityEngine.Physics.Raycast(gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.position, eyeGazeProvider.GazeDirection, out hit))
            {
                mesh.enabled = true;
                line.enabled = true;

                line.SetPosition(0, gameObject.transform.position);
                line.SetPosition(1, gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.position);

                gameObject.transform.position = hit.point;
            }
            else // hit nothing
            {
                mesh.enabled = false;
                line.enabled = false;
            }
        }
    }


}
