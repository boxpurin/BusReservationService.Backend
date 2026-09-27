namespace BusReservationService.Endpoints.User;

using FastEndpoints;

public class CreateUser : Endpoint<CreateUserRequest, CreateUserResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/api/users/create");
    }

    public override async Task HandleAsync(CreateUserRequest req, CancellationToken c)
    {
        var session_id_kv = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "session_id");

        if (session_id_kv.Value == "")
        {
            await Send.UnauthorizedAsync(c);
            return;
        }

        var session_id = session_id_kv.Value;
    }
}

public record CreateUserRequest(string FirstName, string LastName, string PhoneNumber);
public record CreateUserResponse();