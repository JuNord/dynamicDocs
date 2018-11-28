using System;

namespace RestService
{
    public class MissingValueInterpretationException : Exception
    {
        public MissingValueInterpretationException() : base(
            "No Func<bool> was supplied to \"GetValueAsString\" and it cant be interpreted as a string")
        {
        }

        public MissingValueInterpretationException(string message) : base(message)
        {
        }
    }
}