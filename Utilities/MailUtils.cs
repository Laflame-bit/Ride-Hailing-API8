namespace RideHailingAPI.Utilities;

public class MailUtils
{
     public static string Create(string otpCode)
    {
        return $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Email Verification</title>
            </head>

            <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, sans-serif;">

                <div style="max-width:600px; margin:40px auto; background:#ffffff; border-radius:10px; padding:40px;">

                    <h1 style="margin-top:0; color:#222222;">
                        RideHailingAPI
                    </h1>

                    <h2 style="color:#333333;">
                        Verify your email address
                    </h2>

                    <p style="font-size:16px; color:#555555; line-height:1.6;">
                        Thank you for registering with RideHailingAPI.
                        Please use the verification code below to verify your email address.
                    </p>

                    <div style="text-align:center; margin:30px 0;">
                        <div style="
                            display:inline-block;
                            background:#f0f2f5;
                            padding:18px 30px;
                            border-radius:8px;
                            font-size:32px;
                            font-weight:bold;
                            letter-spacing:8px;
                            color:#222222;">
                            {otpCode}
                        </div>
                    </div>

                    <p style="font-size:15px; color:#666666;">
                        This verification code will expire in <strong>5 minutes</strong>.
                    </p>

                    <p style="font-size:14px; color:#888888; line-height:1.5;">
                        If you did not create a RideHailingAPI account, you can safely ignore this email.
                        Do not share this verification code with anyone.
                    </p>

                    <hr style="border:none; border-top:1px solid #eeeeee; margin:30px 0;">

                    <p style="font-size:12px; color:#999999; text-align:center;">
                        © RideHailingAPI. All rights reserved.
                    </p>

                </div>

            </body>
            </html>
            """;
    }

    public static class PasswordResetOtpTemplate
    {
        public static string Create(string otpCode)
        {
            return $"""
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="UTF-8">
                        <meta name="viewport" content="width=device-width, initial-scale=1.0">
                        <title>Password Reset</title>
                    </head>

                    <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, sans-serif;">

                        <div style="max-width:600px; margin:40px auto; background:#ffffff; border-radius:10px; padding:40px;">

                            <h1 style="margin-top:0; color:#222222;">
                                RideHailingAPI
                            </h1>

                            <h2 style="color:#333333;">
                                Password Reset Request
                            </h2>

                            <p style="font-size:16px; color:#555555; line-height:1.6;">
                                We received a request to reset the password for your RideHailingAPI account.
                                Use the code below to continue.
                            </p>

                            <div style="text-align:center; margin:30px 0;">
                                <div style="
                                    display:inline-block;
                                    background:#f0f2f5;
                                    padding:18px 30px;
                                    border-radius:8px;
                                    font-size:32px;
                                    font-weight:bold;
                                    letter-spacing:8px;
                                    color:#222222;">
                                    {otpCode}
                                </div>
                            </div>

                            <p style="font-size:15px; color:#666666;">
                                This password reset code will expire in <strong>5 minutes</strong>.
                            </p>

                            <p style="font-size:14px; color:#888888; line-height:1.5;">
                                If you did not request a password reset, please ignore this email.
                                Do not share this code with anyone.
                            </p>

                            <hr style="border:none; border-top:1px solid #eeeeee; margin:30px 0;">

                            <p style="font-size:12px; color:#999999; text-align:center;">
                                © RideHailingAPI. All rights reserved.
                            </p>

                        </div>

                    </body>
                    </html>
                    """;
        }
    }
}