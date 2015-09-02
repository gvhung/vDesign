namespace Base.UI
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface ITreeNodeIcon
    {
        Icon Icon { get; set; }
    }

    [ComplexType]
    public class Icon
    {
        public string Color { get; set; }
        public string Value { get; set; }
    }
}