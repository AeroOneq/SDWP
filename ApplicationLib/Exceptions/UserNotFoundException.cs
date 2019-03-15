using System;
using System.Runtime.Serialization;

namespace ApplicationLib.Exceptions
{
    /// <summary>
    /// Exception which is thrown when there is en error in finding an user
    /// record in the database
    /// </summary>
    public class UserNotFoundException : SystemException
    {
        public UserNotFoundException() { }
        public UserNotFoundException(string message)
            :base(message) { }
        public UserNotFoundException(string message, Exception innerException) 
            : base(message, innerException) { }
        protected UserNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
