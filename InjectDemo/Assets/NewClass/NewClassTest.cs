using System;
using System.Collections.Generic;
using UnityEngine;
using IFix.Core;
using System.IO;
using System.Threading;
using UpLoad;
using System.Collections;

//1、执行菜单“InjectFix/Fix”生成补丁；
//2、注释“NewBehaviourScript”，“SubSystem2”两个类，以及NewClassTest的Init函数里头new SubSystem2的那行语句；
//3、执行菜单“InjectFix/Inject”，模拟线上没有“NewBehaviourScript”，“SubSystem2”的版本；
//4、NewClassTest.cs拖到场景，运行看下效果，此时只加载SubSystem1；
//5、把生成的补丁拷贝到Resources下，再次运行看下效果；

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

public class SubSystem1 : ISubSystem
{
    public bool running { get { return true; } }

    public void Start()
    {
        Debug.Log("SubSystem1.Start");
    }

    public void Stop()
    {
        Debug.Log("SubSystem1.Stop");
    }

    public void Destroy()
    {
        Debug.Log("SubSystem1.Destroy");
    }
}

/*
//新增类
[IFix.Interpret]
public class NewClass
{
    public Action<object> act;

    public string testField = "TestField";

    public NewClass(string cons)
    {
        UnityEngine.Debug.Log(cons);
    }

    public void Print()
    {
        Debug.LogError("new class print");
    }
}

[IFix.Interpret]
public class NewBehaviourScript : IMonoBehaviour
{
    private int tick = 0;

    public void Start()
    {
        Debug.Log("NewBehaviourScript.Start");
    }

    public void Update()
    {
        if (tick++ % 60 == 0)
        {
            Debug.Log("NewBehaviourScript.Update");
        }
    }
}

[IFix.Interpret]
public class SubSystem2 : ISubSystem
{
    public bool running { get { return true; } }

    public void Start()
    {
        Debug.Log("SubSystem2.Start, create GameObject and attach a NewBehaviourScript");
        var go = new GameObject("hehe");
        var behaviour = go.AddComponent(typeof(VMBehaviourScript)) as VMBehaviourScript;
        behaviour.VMMonoBehaviour = new NewBehaviourScript();
    }

    public void Stop()
    {
        Debug.Log("SubSystem2.Stop");
    }

    public void Destroy()
    {
        Debug.Log("SubSystem2.Destroy");
    }
}
*/


public class NewClassTest : MonoBehaviour
{
    List<ISubSystem> subsystems = new List<ISubSystem>();
    public string testFieldName = "testFieldName";

    private string testProperty= "testProperty";
    public string TestProperty
    {
        get { return testProperty; }
        set
        {
            testProperty = value;
        }
    }

    public delegate void SetVal(int x);
    public event SetVal SetValEvent;

    Position position;

    void Awake()
    {
        UpLoadFiles.Download("/" + Config.fileName, Application.persistentDataPath + "/", Config.fileName);

        string fileName = Application.persistentDataPath + "/"+Config.fileName;
      
        //var patch = Resources.Load<TextAsset>("Assembly-CSharp.patch");
        if (File.Exists(fileName))
        {
            Debug.Log("loading Assembly-CSharp.patch ...");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            PatchManager.Load(new FileStream(fileName,FileMode.Open));
            Debug.Log("patch Assembly-CSharp.patch, using " + sw.ElapsedMilliseconds + " ms");
        }
        Init();
    }

    void Start()
    {
        foreach (var subSystem in subsystems)
        {
            subSystem.Start();
        }
    }

    void OnDestroy()
    {
        SetValEvent -= setEventTest;
        foreach (var subSystem in subsystems)
        {
            subSystem.Destroy();
        }
    }

    private void setEventTest(int x)
    {
        Debug.Log("setEventTest:"+x);
    }

    //[IFix.Patch]
    private void Init()
    {
        Debug.Log("Init");
        subsystems.Add(new SubSystem1());
        //subsystems.Add(new SubSystem2());

        //Config.tmp1 = "tmp2";
        //NewClass newClass = new NewClass(Config.tmp1);

        SetValEvent += setEventTest;
        position = new Position(2,3);
        if(SetValEvent!=null)
        {
            SetValEvent(2);
        }
        StartCoroutine(PatchEnumerator());

        InnerGenericMethod(2);
    }


    public IEnumerator PatchEnumerator()
    {
        yield return new WaitForSeconds(1);
        UnityEngine.Debug.Log("Wait 1 Second");
    }

    public void InnerGenericMethod<T>(T t)
    {
        UnityEngine.Debug.Log("InnerGenericMethod: " + t);
    }


}

struct Position
{
    int x;
    int y;

    public Position(int mx,int my)
    {
        x = mx;
        y = my;
    }
}



[IFix.CustomBridge]
public static class AdditionalBridge
{
    static List<Type> bridge = new List<Type>()
    {
        typeof(ISubSystem),
        typeof(IMonoBehaviour)
    };
}