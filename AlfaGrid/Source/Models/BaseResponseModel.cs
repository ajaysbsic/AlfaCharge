namespace AlfaGrid.Source.Models
{
    public class BaseResponseModel<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}