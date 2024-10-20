namespace Diplomski.Helpers
{
    public class EmailResetPasswordBody
    {
        public string createBody(string input)
        {
            string confirmationLink = "https://localhost:7192/api/User/ResetPassword/" + Uri.EscapeDataString(input);

            string htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Reset Your Password</title>
                </head>
                <body>
                    <h1>Please reset ypur password!</h1>
                    <p>Please reset your password klicking the link below::</p>
                    <p><a href='{confirmationLink}' style='color: #4CAF50; text-decoration: none;'>Reset your password</a></p>
                    <p>If you didn't send this request, you can safely ignore this email.</p>
                    <p>Best Regards,<br>Emrah Jamakovich</p>
                </body>
                </html>
                ";

            return htmlBody;
        }
    }
}
