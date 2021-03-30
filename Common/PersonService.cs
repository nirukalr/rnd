using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class PersonService : IPersonService
    {
        public string GetData()
        {
            return "This is some data from PersonService";
        }
    }
}
