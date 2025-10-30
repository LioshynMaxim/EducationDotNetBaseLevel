using System;
using System.Linq;
using Task3.DoNotChange;
using Task3.Exceptions;

namespace Task3
{
    public class UserTaskService(IUserDao userDao)
    {
        private readonly IUserDao _userDao = userDao;

        public void AddTaskForUser(int userId, UserTask task)
        {
            if (userId < 0)
            {
                throw new InvalidUserIdException();
            }

            var user = _userDao.GetUser(userId) ?? throw new UserNotFoundException();
            var tasks = user.Tasks;
            var hasError = tasks
                .Any(t => string.Equals(task.Description, t.Description, StringComparison.OrdinalIgnoreCase));

            if (hasError)
            {
                throw new TaskAlreadyExistsException();
            }

            tasks.Add(task);
        }
    }
}
