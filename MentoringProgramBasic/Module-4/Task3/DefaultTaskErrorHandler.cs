using System;
using System.Collections.Generic;
using Task3.Exceptions;

namespace Task3
{
    public class DefaultTaskErrorHandler : ITaskErrorHandler
    {
        private readonly Dictionary<Type, Func<Exception, string>> _errorMap;

        public DefaultTaskErrorHandler() => 
            _errorMap = new Dictionary<Type, Func<Exception, string>>
            {
               { typeof(InvalidUserIdException), ex => ex.Message },
               { typeof(UserNotFoundException), ex => ex.Message },
               { typeof(TaskAlreadyExistsException), ex => ex.Message }
            };

        public string HandleError(Exception ex)
        {
            var exceptionType = ex.GetType();
            return _errorMap.TryGetValue(exceptionType, out var handler) 
                ? handler(ex) 
                : "An unexpected error occurred.";
        }
    }
}
