namespace Shapy.Application.Dto
{
    public class ImageGenerationRequest
    {
        public ImageInput Input { get; set; } = new();
    }

    public class ImageInput
    {
        public string Prompt { get; set; } = string.Empty;
        public string Resolution { get; set; } = "None";
        public string Style_Type { get; set; } = "None";
        public string Aspect_Ratio { get; set; } = "3:2";
        public string Magic_Prompt_Option { get; set; } = "Auto";
    }

    public class ReplicateRequest
    {
        public ImageInput Input { get; set; } = new();
    }

    public class ImageGenerationResponse
    {
        public bool Success { get; set; }
        public string? ImageUrl { get; set; }
        public string? Error { get; set; }
    }

    public class ReplicateResponse
    {
        public string? Output { get; set; }
        public string? Status { get; set; }
        public string? Id { get; set; }
    }
}