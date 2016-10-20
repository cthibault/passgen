﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator.Random
{
    interface IRandomProvider
    {
        int Next(int length);
        int Next(byte length);
    }
}
