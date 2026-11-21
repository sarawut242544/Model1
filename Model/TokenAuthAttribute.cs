using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class TokenAuthAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext ctx)
    {
        var h = ctx.HttpContext.Request.Headers["Authorization"].ToString();
        var uid = TokenStore.Validate(h);
        if (uid == null)
            ctx.Result = new OkObjectResult(
                ApiResult.Fail(401, "The token is invalid or expired"));
        else ctx.HttpContext.Items["userId"] = uid.Value;
    }
}
