using System;
using Task3.DoNotChange;

namespace Task3;

public class UserTaskController
{
    private readonly UserTaskService _taskService;
    private readonly ITaskErrorHandler _errorHandler;

    public UserTaskController(UserTaskService taskService)
    {
        _taskService = taskService;
        _errorHandler = new DefaultTaskErrorHandler();
    }

    public UserTaskController(UserTaskService taskService, ITaskErrorHandler errorHandler)
    {
        _taskService = taskService;
        _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
    }

    public bool AddTaskForUser(int userId, string description, IResponseModel model)
    {
        try
        {
            var task = new UserTask(description);
            _taskService.AddTaskForUser(userId, task);
            return true;
        }
        catch (Exception ex)
        {
            string errorMessage = _errorHandler.HandleError(ex);
            model.AddAttribute("action_result", errorMessage);
            return false;
        }
    }
}
