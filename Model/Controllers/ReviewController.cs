using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Models;

[ApiController]
public class ReviewController(MapDbContext db) : ControllerBase
{
    [TokenAuth]
    [HttpGet("system/api/review/list")]
    public ApiResult List(int pageNum = 1, int pageSize = 10,
        string? businessType = null, long? businessId = null)
    {
        var q = from r in db.Reviews
                join u in db.Users on r.UserId equals u.UserId
                select new { r, u.NickName };
        if (!string.IsNullOrEmpty(businessType))
            q = q.Where(x => x.r.BusinessType == businessType);
        if (businessId != null) q = q.Where(x => x.r.BusinessId == businessId);
        int total = q.Count();
        var rows = q.OrderBy(x => x.r.ReviewId)
            .Skip((pageNum - 1) * pageSize).Take(pageSize).ToList()
            .Select(x => new {
                x.r.ReviewId, x.r.UserId, userName = x.NickName,
                x.r.BusinessType, x.r.BusinessId, x.r.LikeCount,
                rating = x.r.Rating.ToString(), x.r.ReviewContent,
                reviewTime = x.r.ReviewTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                imageUrls = x.r.ImageUrls?.Split(',') ?? Array.Empty<string>()
            });
        return ApiResult.Ok(new { total, rows });
    }

    [TokenAuth]
    [HttpGet("system/api/review/mine")]
    public ApiResult Mine(int pageNum = 1, int pageSize = 10,
        string? businessType = null, long? businessId = null)
    {
        int userId = (int)HttpContext.Items["userId"]!;
        var q = from r in db.Reviews
                join u in db.Users on r.UserId equals u.UserId
                where r.UserId == userId
                select new { r, u.NickName };
        if (!string.IsNullOrEmpty(businessType))
            q = q.Where(x => x.r.BusinessType == businessType);
        if (businessId != null) q = q.Where(x => x.r.BusinessId == businessId);
        int total = q.Count();
        var rows = q.OrderBy(x => x.r.ReviewId)
            .Skip((pageNum - 1) * pageSize).Take(pageSize).ToList()
            .Select(x => new {
                x.r.ReviewId, x.r.UserId, userName = x.NickName,
                x.r.BusinessType, x.r.BusinessId, x.r.LikeCount,
                rating = x.r.Rating.ToString(), x.r.ReviewContent,
                reviewTime = x.r.ReviewTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                imageUrls = x.r.ImageUrls?.Split(',') ?? Array.Empty<string>()
            });
        return ApiResult.Ok(new { total, rows });
    }

    [TokenAuth]
    [HttpPost("system/api/review/add")]
    public ApiResult Add(ReviewAddRequest? req)
    {
        if (req == null)
            return ApiResult.Fail(400, "Request body is required");
        if (req.BusinessType != "scenic" && req.BusinessType != "restaurant")
            return ApiResult.Fail(400, "businessType must be scenic or restaurant");
        if (req.BusinessId == null)
            return ApiResult.Fail(400, "businessId is required");
        if (req.Rating == null || req.Rating < 1 || req.Rating > 5)
            return ApiResult.Fail(400, "rating must be between 1 and 5");
        if (string.IsNullOrWhiteSpace(req.ReviewContent))
            return ApiResult.Fail(400, "reviewContent is required");

        int userId = (int)HttpContext.Items["userId"]!;
        db.Reviews.Add(new Review {
            UserId = userId, BusinessType = req.BusinessType,
            BusinessId = req.BusinessId.Value, Rating = req.Rating.Value,
            ReviewContent = req.ReviewContent, ReviewTime = DateTime.Now,
            ImageUrls = req.ImageUrls == null ? null : string.Join(",", req.ImageUrls),
            LikeCount = 0
        });
        return ApiResult.Ok(db.SaveChanges());
    }

    [TokenAuth]
    [HttpDelete("system/api/review/{reviewIds}")]
    public ApiResult Delete(string reviewIds)
    {
        int userId = (int)HttpContext.Items["userId"]!;
        var ids = reviewIds.Split(',')
            .Select(s => int.TryParse(s, out var n) ? n : -1)
            .ToList();
        int deleted = db.Reviews
            .Where(r => r.UserId == userId && ids.Contains(r.ReviewId))
            .ExecuteDelete();
        return ApiResult.Ok(deleted);
    }
}

public class ReviewAddRequest
{
    public string? BusinessType { get; set; }
    public int? BusinessId { get; set; }
    public int? Rating { get; set; }
    public string? ReviewContent { get; set; }
    public List<string>? ImageUrls { get; set; }
}
