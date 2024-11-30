namespace Task_Board_API.Models.ErrorResponses
{
    public class ValidationError
    {
        public ValidationError(Dictionary<string, string> errors)
        {
            this.Errors = errors;
        }
        public string Type { get; set; } = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
        public string Title { get; set; } = "One or more validation errors occurred.";
        public int Status { get; set; } = 400;
        public Dictionary<string, string> Errors { get; set; }
    }
}
