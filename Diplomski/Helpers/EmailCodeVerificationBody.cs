namespace Diplomski.Helpers
{
    public class EmailCodeVerificationBody
    {
        public string CreateBody(string verificationCode)
        {
            string htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Your Verification Code</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f7f7f7;
                            padding: 20px;
                        }}
                        .container {{
                            background-color: #ffffff;
                            border-radius: 8px;
                            padding: 20px;
                            max-width: 600px;
                            margin: auto;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                            color: #333;
                        }}
                        .code {{
                            font-size: 24px;
                            font-weight: bold;
                            color: #4CAF50; /* Green */
                            margin: 20px 0;
                        }}
                        .footer {{
                            font-size: 12px;
                            color: #999;
                            margin-top: 20px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>Your Verification Code</h1>
                        <p>Thank you for signing up! Your verification code is:</p>
                        <div class='code'>{verificationCode}</div>
                        <p>Please enter this code in the application to verify your email address.</p>
                        <p>If you didn't sign up for this account, you can safely ignore this email.</p>
                        <div class='footer'>
                            <p>Best Regards,<br>Emrah Jamakovich</p>
                        </div>
                    </div>
                </body>
                </html>";

            return htmlBody;
        }
    }
}
