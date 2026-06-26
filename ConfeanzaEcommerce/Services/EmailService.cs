using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ConfeanzaEcommerce.Services;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = "";
    public string SenderName { get; set; } = "";
    public string AppPassword { get; set; } = "";
    public string ReceiverEmail { get; set; } = "";
}

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IConfiguration config)
    {
        _settings = config.GetSection("EmailSettings").Get<EmailSettings>()
                    ?? throw new InvalidOperationException("EmailSettings not configured.");
    }

    public async Task SendContactFormAsync(string senderName, string senderEmail, string subject, string message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(new MailboxAddress(_settings.SenderName, _settings.ReceiverEmail));
        email.ReplyTo.Add(new MailboxAddress(senderName, senderEmail));
        email.Subject = $"[ConfeanzaEcommerce Contact] {subject}";

        var body = new BodyBuilder
        {
            HtmlBody = $"""
                <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #fce7f3;border-radius:10px;overflow:hidden;">
                  <div style="background:linear-gradient(135deg,#be185d,#ec4899);padding:24px 32px;">
                    <h2 style="color:#fff;margin:0;">New Contact Form Message</h2>
                    <p style="color:rgba(255,255,255,.8);margin:4px 0 0;">ConfeanzaEcommerce</p>
                  </div>
                  <div style="padding:28px 32px;background:#fff;">
                    <table style="width:100%;border-collapse:collapse;">
                      <tr><td style="padding:8px 0;color:#6b7280;width:120px;font-size:13px;">From</td>
                          <td style="padding:8px 0;font-weight:600;color:#1e1b4b;">{senderName}</td></tr>
                      <tr><td style="padding:8px 0;color:#6b7280;font-size:13px;">Email</td>
                          <td style="padding:8px 0;color:#ec4899;">{senderEmail}</td></tr>
                      <tr><td style="padding:8px 0;color:#6b7280;font-size:13px;">Subject</td>
                          <td style="padding:8px 0;color:#1e1b4b;">{subject}</td></tr>
                    </table>
                    <hr style="border:none;border-top:1px solid #fce7f3;margin:16px 0;" />
                    <p style="color:#6b7280;font-size:13px;margin-bottom:8px;">Message:</p>
                    <p style="color:#374151;line-height:1.7;white-space:pre-line;">{message}</p>
                  </div>
                  <div style="background:#fdf2f8;padding:16px 32px;text-align:center;">
                    <p style="color:#9ca3af;font-size:12px;margin:0;">Reply directly to this email to respond to {senderName}.</p>
                  </div>
                </div>
                """
        };
        email.Body = body.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
        await smtp.SendAsync(email);

        // Send thank-you confirmation to the submitter
        var thanks = new MimeMessage();
        thanks.From.Add(new MailboxAddress("No Reply | ConfeanzaEcommerce", _settings.SenderEmail));
        thanks.To.Add(new MailboxAddress(senderName, senderEmail));
        thanks.Subject = "Thank you for contacting ConfeanzaEcommerce!";

        thanks.Body = new BodyBuilder
        {
            HtmlBody = $"""
                <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #fce7f3;border-radius:10px;overflow:hidden;">
                  <div style="background:linear-gradient(135deg,#be185d,#ec4899);padding:32px;text-align:center;">
                    <div style="font-size:2.5rem;">🛍️</div>
                    <h2 style="color:#fff;margin:8px 0 4px;">Thank You, {senderName}!</h2>
                    <p style="color:rgba(255,255,255,.85);margin:0;font-size:15px;">We've received your message.</p>
                  </div>
                  <div style="padding:32px;background:#fff;">
                    <p style="color:#374151;line-height:1.8;font-size:15px;">
                      Hi <strong>{senderName}</strong>,<br><br>
                      Thank you for reaching out to <strong>ConfeanzaEcommerce</strong>. We have received your enquiry
                      about <em>"{subject}"</em> and our team will get back to you within <strong>1 business day</strong>.
                    </p>
                    <div style="background:#fdf2f8;border:1px solid #fce7f3;border-radius:8px;padding:16px 20px;margin:20px 0;">
                      <p style="color:#6b7280;font-size:13px;margin:0 0 6px;">Your message:</p>
                      <p style="color:#374151;font-size:14px;line-height:1.7;margin:0;white-space:pre-line;">{message}</p>
                    </div>
                    <p style="color:#6b7280;font-size:14px;line-height:1.7;">
                      In the meantime, you can also reach us via:<br>
                      📧 <a href="mailto:confeanzatech.official@gmail.com" style="color:#ec4899;">confeanzatech.official@gmail.com</a><br>
                      💬 <a href="https://wa.me/15055852851" style="color:#ec4899;">WhatsApp</a> &nbsp;|&nbsp;
                      📞 <a href="tel:+15055852851" style="color:#ec4899;">+1 (505) 585-2851</a>
                    </p>
                    <a href="https://ecommerce.confeanzatech.com" style="display:inline-block;margin-top:16px;padding:12px 32px;background:linear-gradient(135deg,#be185d,#ec4899);color:#fff;text-decoration:none;border-radius:25px;font-weight:600;font-size:14px;">
                      Browse Best Deals →
                    </a>
                  </div>
                  <div style="background:#fdf2f8;padding:16px 32px;text-align:center;border-top:1px solid #fce7f3;">
                    <p style="color:#9ca3af;font-size:12px;margin:0;">
                      This is an automated confirmation. Please do not reply to this email.<br>
                      © 2026 ConfeanzaEcommerce — India's Trusted Price Comparison Platform
                    </p>
                  </div>
                </div>
                """
        }.ToMessageBody();

        await smtp.SendAsync(thanks);
        await smtp.DisconnectAsync(true);
    }
}
