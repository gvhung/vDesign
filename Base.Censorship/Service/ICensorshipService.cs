using Base.Service;

namespace Base.Censorship.Service
{
    public interface ICensorshipService: IService
    {
        void CheckObsceneLexis(string message);
    }
}