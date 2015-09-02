namespace Base.Service
{
    public interface IObjectValidationResult
    {
        string Property { get; set; }
        string Error { get; set; }
    }

    public class testObjectValidationResult : IObjectValidationResult
    {

        public string Property
        {
            get;
            set;
        }

        public string Error
        {
            get;
            set;
        }
    }
}