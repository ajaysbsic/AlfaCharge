

namespace AlfaGrid.Source.Models
{
    public class BaseEntity<T> : BaseResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public T Result { get; set; }
    }
}