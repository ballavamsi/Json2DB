using System;
using System.Collections.Generic;
using System.Text;

namespace Json2DB
{
    public class ObjectMapping
    {
        public int ObjectNumber { get; set; } = 1;
        public string APIParamName { get; set; }
        public string DBParameterName { get; set; }
        public string ObjectName { get; set; }
        public string DbType { get; set; } = "string";
        public string Direction { get; set; } = "IN";
        public int ChildObjectNumber { get; set; } = 0;
    }
}
