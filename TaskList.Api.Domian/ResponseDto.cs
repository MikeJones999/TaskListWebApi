namespace TaskList.Api.Domian
{
    public class ResponseDto<T>
    {
        public T? ResponseData { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
