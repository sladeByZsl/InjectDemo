using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Yunchang.Download
{
    public class TestAsyncOperation : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get { return true; }
        }
    }
}
