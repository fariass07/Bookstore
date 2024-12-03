using System.Runtime.Serialization;

namespace Bookstore.Services.Exceptions
{
    [Serializable]
    internal class NotFoundException : ApplicationException
    {
        public NotFoundException(string? message) : base(message)
        {

        }
    }
}