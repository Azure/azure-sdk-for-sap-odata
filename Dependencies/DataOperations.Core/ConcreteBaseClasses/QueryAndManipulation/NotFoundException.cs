using System;

namespace DataOperations
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message, Exception Inner) : base(message, Inner)
        {
            
        }
    }
}