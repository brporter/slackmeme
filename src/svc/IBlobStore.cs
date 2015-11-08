using System;
using System.IO;

namespace BryanPorter.SlackMeme.Service
{
    public interface IBlobStore
    {
        Uri GetUri(string identifier);

        bool Exists(string identifier);

        void Store(string identifier, Stream stream);

        void Delete(string identifier);
    }
}