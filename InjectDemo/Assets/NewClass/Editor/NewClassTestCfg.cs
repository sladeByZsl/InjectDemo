using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFix;
using System;

[Configure]
public class TestAllCfg
{
	[IFix]
    static IEnumerable<Type> hotfix
    {
        get
        {
            return new List<Type>()
            {
                typeof(Config),
                typeof(TestAll),
                typeof(TestModify),
                typeof(Animal),
                typeof(People),
                typeof(IEnumerable),
                typeof(IEnumerator)
            };
        }
    }
}
