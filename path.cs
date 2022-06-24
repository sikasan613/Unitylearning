using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class path : MonoBehaviour
{
    public Color lineColor;
    //中心点
    public List<Transform> nodes = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //AI用の道を線で確認のため作成
    void OnDrawGizmos()
    {
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 carentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;
            if (i > 0)
            {
                previousNode = nodes[i - 1].position;
            }
            else if (i == 0 && nodes.Count > 1)
            {
                previousNode = nodes[nodes.Count - 1].position;
            }
            Gizmos.DrawLine(previousNode, carentNode);
        }
    }
}
