using System.Runtime.Serialization;

namespace Bookstore.Services.Exceptions
{
    [Serializable]
    internal class DbConcorrencyException : ApplicationException
    {
        public DbConcorrencyException(string? message) : base(message)
        {

        }

    }
}