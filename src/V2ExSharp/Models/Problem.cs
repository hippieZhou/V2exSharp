using System.Collections.Generic;
using HtmlAgilityPack;

namespace V2exSharp.Models;

[HasXPath]
public class Problem
{
    [XPath("//div[@class='problem']//li")]
    [SkipNodeNotFound]
    public List<string> Errors { get; set; } = new();

    public bool HasProblem() => Errors.Count > 0;
}
