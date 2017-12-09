﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding
{
    public interface ISerializer
    {
        string SerializeObject(object obj);
        T DeserializeObject<T>(string serializedObj);
    }
}