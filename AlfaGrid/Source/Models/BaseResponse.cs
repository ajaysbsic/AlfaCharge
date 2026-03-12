namespace AlfaGrid.Source.Models
{
    public class BaseResponse
    {
        public bool IsException { get; set; }

        //[Ignore]
        public Exception Exception { get; set; }

    }
}