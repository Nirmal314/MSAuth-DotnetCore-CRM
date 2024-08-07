using MSAuth.Models;

namespace MSAuth.ViewModels;

public class SoftwareInventoryViewModel
{
    public required SoftwareCategory SoftwareCategory { get; set; }
    public List<Software> CurrentSoftwares { get; set; } = [];
    public List<Software> FutureSoftwares { get; set; } = [];
    public List<Software> OtherSoftwares { get; set; } = [];
}
