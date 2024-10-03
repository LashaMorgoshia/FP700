using System;

namespace FP700Win.Events
{
    public class EcrThrewExceptionEvent
    {
        public Exception Exception;

        public EcrThrewExceptionEvent(Exception ex)
        {
            Exception = ex;
        }
    }

}
