using Microsoft.AspNetCore.Mvc;
using Model.Models;

[ApiController]
public class ScenicSpotController(MapDbContext db) : ControllerBase
{
    [HttpGet("system/api/scenicSpot/list")]
    public ApiResult List(int pageNum = 1, int pageSize = 10,
        string? spotName = null, string? address = null)
    {
        var q = db.ScenicSpots.AsQueryable();
        if (!string.IsNullOrEmpty(spotName))
            q = q.Where(x => x.SpotName!.Contains(spotName));
        if (!string.IsNullOrEmpty(address))
            q = q.Where(x => x.Address!.Contains(address));

        int total = q.Count();
        var rows = q.OrderBy(x => x.SpotId)
            .Skip((pageNum - 1) * pageSize).Take(pageSize).ToList()
            .Select(x => new {
                x.SpotId, x.SpotName, x.Address, x.Phone,
                latitude = x.Latitude.ToString(), longitude = x.Longitude.ToString(),
                rating = x.Rating.ToString(), ticketPrice = x.TicketPrice?.ToString(),
                x.OpeningHours, x.Description, x.ImageUrl,
                distance = x.Distance.ToString(),
                businessType = "scenic"
            });
        return ApiResult.Ok(new { total, rows });
    }

    [HttpGet("system/api/scenicSpot/{spotId}")]
    public ApiResult Detail(int spotId)
    {
        var x = db.ScenicSpots.FirstOrDefault(s => s.SpotId == spotId);
        if (x == null)
            return ApiResult.Fail(404, "Scenic spot not found");
        return ApiResult.Ok(new {
            x.TouristPeriod, x.SpotId, x.SpotName, x.Address, x.Phone,
            latitude = x.Latitude.ToString(), longitude = x.Longitude.ToString(),
            rating = x.Rating.ToString(), ticketPrice = x.TicketPrice?.ToString(),
            x.OpeningHours, x.Description,
            distance = x.Distance.ToString(), x.ImageUrl,
            businessType = "scenic"
        });
    }
}
