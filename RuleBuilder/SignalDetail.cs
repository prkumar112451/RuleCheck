using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleBuilder
{
    public class SignalDetail
    {
        public string Signal { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }

        public string Rule { get; set; }

        public bool IsTrue { get; set; }
    }
}
