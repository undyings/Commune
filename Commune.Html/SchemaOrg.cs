using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commune.Html
{
  public class SchemaOrg
  {
    public readonly string PageType;
    public readonly string PageUrl;
    public readonly string Title;
    public readonly string[] ImageUrls;
    public readonly DateTime PublishedTime;
    public readonly DateTime ModifiedTime;
    public readonly string Author;
    public readonly string Organization;
    public readonly string LogoUrl;
    public readonly string Description;

    public SchemaOrg(string pageType, string pageUrl, string title, string[] imageUrls,
      DateTime? publishedTime, DateTime? modifiedTime, string author, string organization,
      string logoUrl, string description)
    {
      this.PageType = pageType;
      this.PageUrl = pageUrl;
      this.Title = title;
      this.ImageUrls = imageUrls;
      this.PublishedTime = publishedTime ?? DateTime.UtcNow;
      this.ModifiedTime = modifiedTime ?? PublishedTime;
      this.Author = author;
      this.Organization = organization;
      this.LogoUrl = logoUrl;
      this.Description = description;
    }
  }
}
