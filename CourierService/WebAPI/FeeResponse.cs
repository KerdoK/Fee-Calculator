namespace WebAPI;

public class FeeResponse
{
    public double Fee { get; set; }
    public string Currency { get; set; } = default!;
    public string Message { get; set; } = default!;
}