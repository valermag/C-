using System;


namespace WindowsService
{
    public interface IFileService
    {
        void Run(string sourceDirectory);
    }
}
