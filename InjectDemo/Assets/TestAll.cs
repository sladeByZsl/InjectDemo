using IFix.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UpLoad;

public class TestAll : MonoBehaviour
{
    private bool isUpdate = true;
    void Awake()
    {
        Debug.Log("init awake");
        init();
    }

    private void init()
    {
        UpLoadFiles.Download("/" + Config.fileName, Application.persistentDataPath + "/", Config.fileName);
        string fileName = Application.persistentDataPath + "/" + Config.fileName;
        if (File.Exists(fileName))
        {
            Debug.Log("loading Assembly-CSharp.patch ...");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            PatchManager.Load(new FileStream(fileName, FileMode.Open));
            Debug.Log("patch Assembly-CSharp.patch, using " + sw.ElapsedMilliseconds + " ms");
        }
    }

    void Start()
    {
        Debug.Log("init awake");
    }

    void Update()
    {
        if(isUpdate)
        {
            Debug.Log("init update");
            isUpdate = false;
        }
    }

    void OnDestroy()
    {
        Debug.Log("init destroy");
    }

    public IEnumerator PatchEnumerator()
    {
        yield return new WaitForSeconds(1);
        UnityEngine.Debug.Log("Wait 1 Second");
    }
}


public abstract class Animal
{
    public abstract void eat();
}

public class People : Animal
{
    public override void eat()
    {
        Debug.Log("people eat");
    }

    public virtual void Walk()
    {
        Debug.Log("people walk");
    }
}

public class TestModify:People
{
    public string testField = "TestField";

    public delegate void SetVal(int x);
    public event SetVal SetValEvent;

    public System.Action action;

    public string TestProperty
    {
        get
        {
            return testField;
        }
        set
        {
            testField = value;
        }
    }

    public TestModify()
    {
        Debug.Log("testModify 0 param:"+ testField);
    }

    public TestModify(string x)
    {
        Debug.Log("testModify 1 param:" + x+ ",filedName:"+ testField);
    }

    public void Add(int x,int y)
    {
        int result = x * y;
        Debug.Log("testModify x+y:"+result);
    }

    public void Sub(int x, int y)
    {
        int result = x - y;
        Debug.Log("testModify x+y:" + result);
    }

    public static void Print(string name)
    {
        Debug.Log(name);
    }

    public void SetEvent(SetVal setVal)
    {
        SetValEvent += setVal;
    }

    public void RaiseEvent(int x)
    {
        if(SetValEvent!=null)
        {
            SetValEvent(x);
        }
    }

    public void ClearEvent(SetVal setVal)
    {
        SetValEvent -= setVal;
    }

    public override void Walk()
    {
        Debug.Log("another walk");
    }

    public void PushAction(int delay,System.Action action)
    {
        Debug.Log("PushAction:" + delay);
        if(action!=null)
        {
            action();
        }
    }


    public void InnerGenericMethod<T>(T t)
    {
        Debug.Log("InnerGenericMethod: " + t);
    }
}




