using Microsoft.AspNetCore.Mvc;
using Model.Models;

[ApiController]
public class AuthController(MapDbContext db) : ControllerBase
{
    [HttpPost("api/auth/login")]
    public ApiResult Login(LoginRequest? req)
    {
        if (req?.Username == null || req.Password == null)
            return ApiResult.Fail(401, "Invalid username or password");

        var u = db.Users.FirstOrDefault(x =>
            x.Username == req.Username && x.Password == req.Password);
        if (u == null)
            return ApiResult.Fail(401, "Invalid username or password");

        return ApiResult.Ok(new {
            token = TokenStore.Issue(u.UserId), expiresIn = 3600,
            user = new {
                userId = u.UserId, username = u.Username,
                nickName = u.NickName, email = u.Email,
                phone = u.Phone, avatar = u.Avatar,
                createTime = u.CreateTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
            }
        });
    }
}

public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
