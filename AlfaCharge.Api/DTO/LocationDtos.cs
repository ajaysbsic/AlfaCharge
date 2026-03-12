using System.ComponentModel.DataAnnotations;

namespace AlfaCharge.Api.DTO;

/// <summary>
/// Location list item DTO.
/// </summary>
public sealed class LocationListDto
{
    public Guid Id { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string? LocationName { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? BusinessName { get; set; }
    public int StationCount { get; set; }
    public int AvailableConnectors { get; set; }
    public int ChargingConnectors { get; set; }
}

/// <summary>
/// Location create/update DTO.
/// </summary>
public sealed class LocationUpsertDto
{
    [Required, MaxLength(128)]
    public string LocationId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? LocationName { get; set; }

    [MaxLength(128)]
    public string? City { get; set; }

    [MaxLength(128)]
    public string? Country { get; set; }

    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }

    [MaxLength(256)]
    public string? BusinessName { get; set; }

    [MaxLength(256)]
    public string? BusinessOwner { get; set; }
}
