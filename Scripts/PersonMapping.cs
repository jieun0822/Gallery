using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class PersonInfo
{
    public int index; // i��
    public int rank; // ���ʺ��� ������� 0
    public int prevRank;
}

public class PersonMapping : MonoBehaviour
{
    public XRInferRVM rvm;
    public List<PersonInfo> personList = new List<PersonInfo> ();
    private bool isFirst = true;
    public bool isMapping = false;

    // Update is called once per frame
    void Update()
    {
        if (isFirst && rvm.GetBodyCount != 0 && rvm.JointsList(0) != null)
        {
            isFirst = false;
            //StartCoroutine(coMapping());
            mappingBody();
        }
    }

    private IEnumerator coMapping()
    {
        yield return null;
        mappingBody();
    }

    private void mappingBody()
    {
        Debug.Log("hi");
        Debug.Log(rvm.GetBodyCount);
        if (personList.Count == 0)
        {
            for (int i = 0; i < 20; i++)
            {
                PersonInfo personInfo = new PersonInfo();
                personInfo.index = i;
                personInfo.rank = -1;
                personInfo.prevRank = -1;
                personList.Add(personInfo);
            }
        }

        // ���� ���� �� �����͸� ������ ����Ʈ
        List<(float value, int bodyIndex)> bodies = new();
        for (int i = 0; i < rvm.GetBodyCount; i++)
        {
            if (rvm.JointsList(i) == null) Debug.Log(i + "null");
            var list = rvm.JointsList(i);
            bodies.Add((list[0].x, i));
        }

        bodies.Sort((a, b) => a.value.CompareTo(b.value));
        bool isChanged = false;
        int rank = 0;
        foreach (var body in bodies)
        {
            int index = body.bodyIndex;

            // ���� ������ ���� prevNum ���� ���
            personList[index].prevRank = personList[index].rank;
            personList[index].rank = rank;
            if (personList[index].rank != personList[index].prevRank)
            {
                isChanged = true;
            }
            rank++;
        }

        if (isChanged)
        {
            Debug.Log("�ٲ���");

            List<(float rank, int bodyIndex)> ranks = new();
            for (int i = 0; i < rvm.GetBodyCount; i++)
            {
                ranks.Add((personList[i].rank, personList[i].index));
            }
            ranks.Sort((a, b) => a.rank.CompareTo(b.rank));

            for (int i = 0; i < ranks.Count; i++)
            {
                Debug.Log(ranks[i].rank + "�� ��� �ε����� " + ranks[i].bodyIndex);
                Debug.Log("���� �ε��� " + rvm.BodyIndex(ranks[i].bodyIndex));
            }
        }

        isMapping = true;
    }

    public int GetBodyIndex(int rank)
    {
        foreach(var info in personList)
        {
            if (info.rank == rank)
            {
                return rvm.BodyIndex(info.index);
            }
        }

        return -1;
    }
}
