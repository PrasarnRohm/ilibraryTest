using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iLibrary
{
    public class iLibraryException
        :Exception
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
