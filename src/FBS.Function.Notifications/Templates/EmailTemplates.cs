namespace FBS.Function.Notifications.Templates;

public static class EmailTemplates
{
    private static string BaseTemplate(string title, string emoji, string content, string bgColor = "#4CAF50")
    {
        return $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>{title}</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        overflow: hidden;
                        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                    }}
                    .header {{
                        background-color: {bgColor};
                        color: white;
                        padding: 30px 20px;
                        text-align: center;
                    }}
                    .header h1 {{
                        margin: 0;
                        font-size: 28px;
                    }}
                    .header .emoji {{
                        font-size: 48px;
                        margin-bottom: 10px;
                    }}
                    .content {{
                        padding: 30px;
                    }}
                    .info-box {{
                        background-color: #f9f9f9;
                        border-left: 4px solid {bgColor};
                        padding: 15px;
                        margin: 20px 0;
                    }}
                    .info-box ul {{
                        list-style: none;
                        padding: 0;
                        margin: 0;
                    }}
                    .info-box li {{
                        padding: 8px 0;
                    }}
                    .info-box strong {{
                        color: #555;
                        display: inline-block;
                        min-width: 120px;
                    }}
                    .alert-box {{
                        background-color: #fff3cd;
                        border-left: 4px solid #ffc107;
                        padding: 15px;
                        margin: 20px 0;
                    }}
                    .footer {{
                        background-color: #f9f9f9;
                        text-align: center;
                        padding: 20px;
                        font-size: 12px;
                        color: #666;
                        border-top: 1px solid #eee;
                    }}
                    .footer p {{
                        margin: 5px 0;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""header"">
                        <div class=""emoji"">{emoji}</div>
                        <h1>{title}</h1>
                    </div>
                    <div class=""content"">
                        {content}
                    </div>
                    <div class=""footer"">
                        <p><strong>Flight Booking System</strong></p>
                        <p>Automated Notification Service</p>
                        <p>&copy; {DateTime.UtcNow.Year} Flight Booking System. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>
        ";
    }

    public static string ReservationCreated(
        string passengerName,
        string flightNumber,
        string seatNumber,
        DateTime expiresAt)
    {
        var expiresAtLocal = expiresAt.ToLocalTime();
        var minutesRemaining = (int)(expiresAt - DateTime.UtcNow).TotalMinutes;

        var content = $@"
            <p>Dear <strong>{passengerName}</strong>,</p>
            <p>Your flight reservation has been <strong>successfully created</strong>! 🎉</p>
            
            <div class=""info-box"">
                <ul>
                    <li><strong>Flight Number:</strong> {flightNumber}</li>
                    <li><strong>Seat Number:</strong> {seatNumber}</li>
                    <li><strong>Expires At:</strong> {expiresAtLocal:yyyy-MM-dd HH:mm} (Local Time)</li>
                </ul>
            </div>

            <div class=""alert-box"">
                <p><strong>⚠️ IMPORTANT - Action Required!</strong></p>
                <p>Your reservation will expire in approximately <strong>{minutesRemaining} minutes</strong>.</p>
                <p>Please confirm your reservation before it expires, or it will be automatically cancelled.</p>
            </div>

            <p>Thank you for choosing our service!</p>
            <p>Safe travels! ✈️</p>";

        return BaseTemplate("Reservation Created", "✈️", content, "#4CAF50");
    }

    public static string ReservationConfirmed(
        string passengerName,
        string flightNumber,
        string seatNumber)
    {
        var content = $@"
            <p>Dear <strong>{passengerName}</strong>,</p>
            <p>Great news! Your reservation has been <strong>confirmed</strong>! ✅</p>
            
            <div class=""info-box"">
                <ul>
                    <li><strong>Flight Number:</strong> {flightNumber}</li>
                    <li><strong>Seat Number:</strong> {seatNumber}</li>
                </ul>
            </div>

            <p>You're all set! Your seat is now reserved and you can proceed with your travel plans.</p>
            <p>We look forward to serving you on this flight!</p>
            <p>Safe travels! ✈️</p>";

        return BaseTemplate("Reservation Confirmed", "✅", content, "#2196F3");
    }

    public static string ReservationCancelled(
        string passengerName,
        string flightNumber,
        string seatNumber)
    {
        var content = $@"
            <p>Dear <strong>{passengerName}</strong>,</p>
            <p>This is to confirm that your reservation has been <strong>cancelled</strong>.</p>
            
            <div class=""info-box"">
                <ul>
                    <li><strong>Flight Number:</strong> {flightNumber}</li>
                    <li><strong>Seat Number:</strong> {seatNumber}</li>
                    <li><strong>Cancelled At:</strong> {DateTime.UtcNow.ToLocalTime():yyyy-MM-dd HH:mm} (Local Time)</li>
                </ul>
            </div>

            <p>The seat has been released and is now available for other passengers.</p>
            <p>If you cancelled by mistake or need assistance, please contact our support team.</p>
            <p>We hope to serve you again in the future!</p>";

        return BaseTemplate("Reservation Cancelled", "❌", content, "#f44336");
    }

    public static string ReservationExpired(
        string passengerName,
        string flightNumber,
        string seatNumber)
    {
        var content = $@"
            <p>Dear <strong>{passengerName}</strong>,</p>
            <p>We regret to inform you that your reservation has <strong>expired</strong>.</p>
            
            <div class=""info-box"">
                <ul>
                    <li><strong>Flight Number:</strong> {flightNumber}</li>
                    <li><strong>Seat Number:</strong> {seatNumber}</li>
                    <li><strong>Expired At:</strong> {DateTime.UtcNow.ToLocalTime():yyyy-MM-dd HH:mm} (Local Time)</li>
                </ul>
            </div>

            <div class=""alert-box"">
                <p><strong>⏰ What happened?</strong></p>
                <p>Your reservation was not confirmed within the 10-minute time window and has been automatically cancelled.</p>
                <p>The seat has been released and is now available for other passengers.</p>
            </div>

            <p>If you still need to book this flight, please create a new reservation.</p>
            <p>We apologize for any inconvenience and hope to serve you in the future!</p>";

        return BaseTemplate("Reservation Expired", "⏰", content, "#FF9800");
    }
}