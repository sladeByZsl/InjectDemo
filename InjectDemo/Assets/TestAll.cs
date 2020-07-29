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
    DownloadAsyncOperation m_DownloadOperation;

    float m_StartTime;

    void Awake()
    {
        Debug.Log("init awake");
        //init();
        init3();
        //testAsyncOperation = new TestAsyncOperation();
        //init4();
    }



    private void init4()
    {
        this.StartCoroutine(tet());
    }

    TestAsyncOperation testAsyncOperation;
    private IEnumerator tet()
    {
        Debug.Log("0");
        yield return new DownloadAsyncOperation();
        Debug.Log("1");
    }

    private void init3()
    {
        this.StartCoroutine(NewDownloadFiles());
    }

    private IEnumerator NewDownloadFiles()
    {
        m_StartTime = 0.0f;
        DownloadInfo[] downloadInfos = LoadDownloadInfos();
        m_DownloadOperation = DownloadAsyncOperation.Start<UnityWebRequestHandler>(downloadInfos, 4);
        m_DownloadOperation.completed += OnDownloadCompleted;
        yield return m_DownloadOperation;
        Debug.LogError("here---------------------------");
        yield return 0;
    }

    private void OnDownloadCompleted(DownloadAsyncOperation obj)
    {
        Debug.LogError("下载成功");
        m_DownloadOperation.completed -= OnDownloadCompleted;
        m_DownloadOperation.Cancel();
        m_DownloadOperation = null;
    }

    private DownloadInfo[] LoadDownloadInfos()
    {
        List<DownloadInfo> tasks = new List<DownloadInfo>();
        Debug.Log("file:"+Application.persistentDataPath + "/" + Config.fileName);
        DownloadInfo ti = new DownloadInfo
        {
            name = Config.fileName,
            url = "http://10.0.107.63/downloads/zsl/" + Config.fileName,
            path = Application.persistentDataPath +"/"+ Config.fileName,
        };
        tasks.Add(ti);
        return tasks.ToArray();
    }

    void Update()
    {
        if (m_DownloadOperation == null)
            return;

        long size = 0;
        foreach (var task in m_DownloadOperation.downloadHandles)
        {
            size += task.info.currentSize;
            if (task.state == DownloadState.downloading)
            {
                Log(task);
            }
        }
        float time = Time.unscaledTime - m_StartTime;
        Debug.Log(string.Format("下载进度: {0:f2}%,下载量: {1:f2}M,下载耗时: {2:f2} (秒),下载速度:{3:f2}K", m_DownloadOperation.Progress * 100, size / (1024 * 1024), time, size / time / 1024));
    }


    void Log(IDownloadHandler handler)
    {
        if (handler.progress < 1.0f)
        {  
            //Debug.Log("<color=red>"+",proces:"+handler.progress+","+ handler.info.name + " - " + handler.info.currentSize+",totalSize:"+handler.info.totalSize + "</color>");
        }
        else
        {
            //Debug.Log("<color=green>" + handler.info.name + " - " + handler.info.currentSize + "</color>");
        }
    }

    void OnDestroy()
    {
        Debug.Log("init destroy");
        this.StopCoroutine(NewDownloadFiles());
        if (m_DownloadOperation != null)
            m_DownloadOperation.Cancel();
        
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

        string saveFilePath = Application.persistentDataPath + "/" + Config.fileName;

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
            Debug.Log("download success");
            LoadPatch();
        }
    }

    //[IFix.Patch]
    private void LoadPatch()
    {
        string fileName = Application.persistentDataPath + "/" + Config.fileName;
        if (File.Exists(fileName))
        {
            Debug.Log("loading Assembly-CSharp.patch ...");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            PatchManager.Load(new FileStream(fileName, FileMode.Open));
            Debug.Log("patch Assembly-CSharp.patch, using " + sw.ElapsedMilliseconds + " ms");
        }
        StartTest();
    }

    //[IFix.Patch]
    private void init()
    {
        //UpLoadFiles.Download("/" + Config.fileName, Application.persistentDataPath + "/", Config.fileName);
        //LoadPatch();
    }


    [IFix.Patch]
    public void Error0()
    {
        var t0 = typeof(Nullable<>);
        var t1 = typeof(List<>);
        int i = 0;
    }

    private void Error0Test(int? a, List<int> b)
    {

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
    //void Update()
    //{
    //    if (isUpdate)
    //    {
    //        Debug.Log("init update");
    //        //testAdd("haha");
    //        //StartCoroutine(PatchEnumerator("abv"));
    //        isUpdate = false;
    //    }
    //}

    void StartTest()
    {
        Debug.Log("StartTest");
    }

    //[IFix.Interpret]
    //public void testAdd(string abc)
    //{
    //    UnityEngine.Debug.Log("testAdd : " + abc);
    //}


    private SortedDictionary<string, List<string>> serverDic = new SortedDictionary<string, List<string>>();
    private List<int> testList = new List<int>();
    //[IFix.Patch]
    // 协程中访问迭代器
    private IEnumerator GetIps()
    {
        //通过迭代器访问无法Patch
        {
            var eList = testList.GetEnumerator();
            var eDic = serverDic.GetEnumerator();
            foreach (var e in serverDic.Keys)
            {
            }
        }

        /* 这种访问没有问题
        {
            for (int i = 0; i < testList.Count; ++i)
            {
                var a = testList[i];
            }

            var keys = new List<string>(serverDic.Keys);
            var values = new List<List<string>>(serverDic.Values);
            for (int i = 0; i < keys.Count; ++i)
            {
                serverDic[keys[i]] = values[i];
            }

            // 通过函数在协程外访问没有问题
            loopAdapter(testList, (e) => { var ee = e; });
            loopAdapter(serverDic, (e) => { return e; });
        }*/

        yield return null;
    }

    private void loopAdapter(List<int> list, Action<int> fun)
    {
        for ( var e = list.GetEnumerator(); e.MoveNext(); )
        {
            fun(e.Current);
        }
    }
    private void loopAdapter(SortedDictionary<string, List<string>> dic, Func<List<string>, List<string>> fun)
    {
        for (var e = dic.GetEnumerator(); e.MoveNext();)
        {
            var newValue = fun(e.Current.Value);
        }
    }


    //[IFix.Patch]
    // 匿名函数中使用KeyValuePair无法Patch
    private void Error3(KeyValuePair<int, int> areaItem)
    {
        // 匿名函数中使用KeyValuePair 无法Patch
        {
            Action fun = () =>
            {
                var kk = areaItem;
            };
        }

        // 可以通过以下方法绕过
        /*{
            // 不适用匿名函数
            Fun(areaItem);

            // 将变量取出
            {

                var k = areaItem.Key;
                var v = areaItem.Value;
                
                // 这种不可以
                //var kv = areaItem;
                Action fun = () =>
                {
                    var kk = k;
                    var vv = v;

                    // 这种不可以
                    //var kkvv = kv;
                };
            }
        }*/
    }
    private void Fun(KeyValuePair<int, int> areaItem)
    {
        var v = areaItem;
    }

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
        //typeof(IEnumerator<int>),
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



