using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace Diplomski.Helpers
{
    public class EmailVerificationBody
    {
            public string createBody(string input)
            {
                string confirmationLink = "https://localhost:7192/api/User/VerifyEmail/" + Uri.EscapeDataString(input);

                string htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Confirm Your Email Address</title>
                </head>
                <body>
                    <h1>Welcome!</h1>
                    <p>Thank you for signing up. Please confirm your email address by clicking the link below:</p>
                    <p><a href='{confirmationLink}' style='color: #4CAF50; text-decoration: none;'>Confirm Email Address</a></p>
                    <p>If you didn't sign up for this account, you can safely ignore this email.</p>
                    <p>Best Regards,<br>Emrah Jamakovich</p>
                </body>
                </html>
                ";

                return htmlBody;
            }
    }
}
