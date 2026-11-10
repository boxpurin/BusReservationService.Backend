namespace BusReservationService.Share.Utils.Error;

public class Error
{
    public string Message { get; init; } = string.Empty;
    public string ErrorCode { get; init; } = string.Empty;
    public int StatusCode { get; init; } = 500;
}