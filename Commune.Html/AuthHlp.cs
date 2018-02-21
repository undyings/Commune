using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Net;
using System.Net.Mail;
using Commune.Basis;

namespace Commune.Html
{
  public static class AuthHlp
  {
    public static string UserName(this HttpContext context)
    {
      if (context != null && context.User != null && context.User.Identity != null)
        return context.User.Identity.Name;
      return null;
    }

    public static bool IsInRole(this HttpContext context, string role)
    {
      if (context != null && context.User != null && context.User.Identity != null)
        return context.User.IsInRole(role);
      return false;
    }

    public static void SetUserFromCookie(this HttpContext context)
    {
      HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
      if (cookie != null)
      {
        try
        {
          if (StringHlp.IsEmpty(cookie.Value))
            return;

          var authTicket = FormsAuthentication.Decrypt(cookie.Value);
          string[] roles = new string[0];
          if (!StringHlp.IsEmpty(authTicket.UserData))
            roles = authTicket.UserData.Split(',');

          SetUser(context, authTicket.Name, roles);
        }
        catch
        {
          //Logger.WriteException(ex);
        }
      }
    }

    public static void Logout(this HttpContext context)
    {
      FormsAuthentication.SignOut();
      context.User = null;
    }

    public static void SetUser(HttpContext context, string login, params string[] roles)
    {
      context.User = new System.Security.Principal.GenericPrincipal(
        new System.Security.Principal.GenericIdentity(login), roles);
    }

    public static void SetUserAndCookie(this HttpContext context, string login, params string[] roles)
    {
      UserInCookie(context, login, roles);
      SetUser(context, login, roles);
    }

    public static void UserInCookie(HttpContext context, string login, params string[] roles)
    {
      FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
        (
           1, //version
           login, // user name
           DateTime.Now,             //creation
           DateTime.Now.AddMonths(1), //Expiration (you can set it to 1 month
           true,  //Persistent
           string.Join(",", roles)
        ); // additional informations

      string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

      HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName,
        encryptedTicket);

      authCookie.Expires = authTicket.Expiration;
      authCookie.HttpOnly = true;

      context.Response.SetCookie(authCookie);
    }

    public static void SendMail(SmtpClient client, string from, string mailto,
      string caption, string messageAsHtml, Attachment attach = null)
    {
      using (MailMessage mail = new MailMessage())
      {
        mail.From = new MailAddress(from);
        mail.To.Add(new MailAddress(mailto));
        mail.Subject = caption;
        mail.BodyEncoding = System.Text.Encoding.UTF8;
        mail.Body = messageAsHtml;
        mail.IsBodyHtml = true;
        if (attach != null)
          mail.Attachments.Add(attach);

        client.Send(mail);
      }
    }

    public static SmtpClient CreateSmtpClient(string smtpServer, int smtpPort,
      string userName, string password)
    {
      SmtpClient client = new SmtpClient(smtpServer);
      if (smtpPort != 0)
        client.Port = smtpPort;
      if (!StringHlp.IsEmpty(userName))
        client.Credentials = new NetworkCredential(userName, password);
      client.DeliveryMethod = SmtpDeliveryMethod.Network;
      return client;
    }
  }
}
