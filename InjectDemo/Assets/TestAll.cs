using FunPlus.Common.Update;
using IFix.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UpLoad;
using Yunchang.Download;

public class TestAll : MonoBehaviour
{
    private bool isUpdate = true;
    TestModify testModify;

    HttpDownLoad mDownLoader;

    void Awake()
    {
        Debug.Log("init awake");
        //init();
        init2();
    }



    private void init2()
    {
        this.StartCoroutine(DownloadFiles());
    }

    private IEnumerator DownloadFiles()
    {
        if (mDownLoader == null)
        {
            mDownLoader = new HttpDownLoad();
        }
        mDownLoader.Init();
        string fileListPath = "http://10.0.107.63/downloads/zsl/"+Config.fileName;

#if UNITY_EDITOR
        string saveFilePath = "Assets/../Download/"+Config.fileName;
#else
        string saveFilePath =  Application.persistentDataPath + "/" + Config.fileName;
#endif
        mDownLoader.DownLoad(fileListPath, saveFilePath);
        while (!mDownLoader.isDone)
        {
            yield return 0;
        }
        if (mDownLoader.error)
        {
            Debug.LogError("download error!!!!");
            yield break;
        }
        else
        {
            Debug.LogError("download success");
        }
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

    //[IFix.Patch]
    void Start()
    {
        Debug.Log("init Start 2");
        //TestModify testModify2 = new TestModify();
        //testModify2.Add(2, 3);
        //testModify2.PushAction(2, () =>
        //{
        //    Debug.Log("push me"+ testModify2.TestProperty);
        //    testModify2.Walk();
        //    TestModify.Print("zsl");
        //});

        //Type type = GetNullableType(typeof(TestAll));
        //Debug.Log("type:"+type.ToString());

        //StartCoroutine(PatchEnumerator(1));

        //StartCoroutine("PatchEnumerator");
        Debug.Log("init Start 3");
        //AddClass addClass = new AddClass(2,3);
        //addClass.handle();
    }

    //[IFix.Patch]
    void Update()
    {
        if (isUpdate)
        {
            Debug.Log("init update");
            //testAdd("haha");
            //StartCoroutine(PatchEnumerator("abv"));
            isUpdate = false;
        }
    }

    void OnDestroy()
    {
        Debug.Log("init destroy");
    }

    //[IFix.Interpret]
    //public void testAdd(string abv)
    //{
    //    UnityEngine.Debug.Log("testAdd : " + abv);
    //}

    //[IFix.Interpret]
    //public IEnumerator<int> PatchEnumerator(int i)
    //{
    //    int m = 0;
    //    yield return 1;
    //    m = 1;
    //}

    //[IFix.Interpret]
    //public IEnumerator PatchEnumerator2(int i)
    //{
    //    UnityEngine.Debug.Log("Wait 1 Second");
    //    yield return new WaitForSeconds(1);
    //    UnityEngine.Debug.Log("Wait 2 Second");
    //}


    //[IFix.Interpret]
    //public IEnumerator PatchEnumerator3(string str)
    //{
    //    UnityEngine.Debug.Log("Wait str 1 Second");
    //    yield return new WaitForSeconds(1);
    //    UnityEngine.Debug.Log("Wait str 2 Second");
    //    yield break;
    //}

    //[IFix.Interpret]
    //public static Type GetNullableType(Type t)
    //{
    //    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
    //    {
    //        Type[] ts = t.GetGenericArguments();
    //        t = ts[0];
    //    }

    //    return t;
    //}
}


public interface IMonoBehaviour
{
    void Start();//简单demo，只定义了Start方法，实际Awake，Update，OnDestroy。。。都类似

    void Update();
}

public interface ISubSystem
{
    bool running { get; }

    void Destroy();

    void Start();

    void Stop();
}

[IFix.CustomBridge]
public static class AdditionalBridge
{
    static List<Type> bridge = new List<Type>()
    {
        typeof(ISubSystem),
        typeof(IMonoBehaviour),
        typeof(IEnumerator<int>),
    };
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


public class TestModify : People
{
    public string testField = "TestField";

    public delegate void SetVal(int x);
    public event SetVal SetValEvent;

    public System.Action action;

    public string TestProperty
    {
        //[IFix.Patch]
        get
        {
            return "123";
            //return testField;
        }
        set
        {
            testField = value;
        }
    }

    public TestModify()
    {
        Debug.Log("testModify 0 param:" + testField);
    }

    public TestModify(string x)
    {
        Debug.Log("testModify 1 param:" + x + ",filedName:" + testField);
    }
    //[IFix.Interpret]
    public void Add(int x, int y, int z = 10)
    {
        int result = x * y * z;
        Debug.Log("testModify x+y:" + result);
    }

    public void Sub(int x, int y)
    {
        int result = x - y;
        Debug.Log("testModify x+y:" + result);
    }

    //[IFix.Patch]
    public static void Print(string name)
    {
        Debug.Log(name + "22222222222");
    }

    public void SetEvent(SetVal setVal)
    {
        SetValEvent += setVal;
    }

    public void RaiseEvent(int x)
    {
        if (SetValEvent != null)
        {
            SetValEvent(x);
        }
    }

    public void ClearEvent(SetVal setVal)
    {
        SetValEvent -= setVal;
    }

    //[IFix.Patch]
    public override void Walk()
    {
        Debug.Log("another walk 2");
    }

    public void PushAction(int delay, System.Action action)
    {
        Debug.Log("PushAction:" + delay);
        if (action != null)
        {
            action();
        }
    }


    public void InnerGenericMethod<T>(T t)
    {
        Debug.Log("InnerGenericMethod: " + t);
    }
}



