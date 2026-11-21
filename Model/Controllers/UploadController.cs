using Microsoft.AspNetCore.Mvc;

[ApiController]
public class UploadController : ControllerBase
{
    [HttpPost("api/upload/images")]
    public ApiResult Images(List<IFormFile>? files)
    {
        if (files == null || files.Count == 0)
            return ApiResult.Fail(400, "files are required");

        var day = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "upload", day);
        Directory.CreateDirectory(dir);

        var details = new List<object>();
        int ok = 0, fail = 0, i = 0;
        foreach (var f in files)
        {
            try
            {
                var name = (DateTime.Now.Ticks + i++) + Path.GetExtension(f.FileName);
                using var s = System.IO.File.Create(Path.Combine(dir, name));
                f.CopyTo(s);
                var rel = "/upload/" + day + "/" + name;
                details.Add(new { fileName = rel, url = Request.Scheme + "://" + Request.Host + rel, size = f.Length });
                ok++;
            }
            catch { fail++; }
        }

        return new ApiResult {
            Code = 200, Msg = "Upload completed",
            Data = new { success = ok, fail, total = files.Count, details }
        };
    }
}
