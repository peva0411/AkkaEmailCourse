﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail
{
    public class Messages
    {
        public class ContinueProcessing
        {
            
        }

        public class InputSuccess
        {
            public string Reason { get; private set; }

            public InputSuccess(string reason)
            {
                Reason = reason;
            }
        }

        public class InputError
        {
            public string Reason { get; private set; }

            public InputError(string reason)
            {
                Reason = reason;
            }
        }
        
        public class NullInputError : InputError
        {
            public NullInputError(string reason) : base(reason)
            {
            }
        }

        public class ValidationError :InputError
        {
            public ValidationError(string reason) : base(reason)
            {
            }
        }

    }
}
