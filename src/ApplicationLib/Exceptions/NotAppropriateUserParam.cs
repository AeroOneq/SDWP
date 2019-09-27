using System;
using System.Runtime.Serialization;

namespace ApplicationLib.Exceptions
{
    /// <summary>
    /// Exception which is thrown when the pararmeter of user class is incorrect
    /// </summary>
    public class NotAppropriateUserParam : SystemException
    {
        public NotAppropriateUserParam() { }
        public NotAppropriateUserParam(string message) : base(message) { }
        public NotAppropriateUserParam(string message, Exception innerException) 
            : base(message, innerException) { }
        protected NotAppropriateUserParam(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
