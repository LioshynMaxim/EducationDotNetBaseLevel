using System;

namespace Task3;

public interface ITaskErrorHandler
{
    string HandleError(Exception ex);
}
