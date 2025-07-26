namespace SSO.EndPoint.WebApi.Extensions;

public static class HTMLPageExtensions
{
    public static string GenerateStatusHtml(string title, string gradientFrom, string gradientTo, string baseUrl, DateTime dateTime)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }}

        body {{
            height: 100vh;
            background: linear-gradient(180deg, {gradientFrom} 0%, {gradientTo} 100%);
            display: flex;
            justify-content: center;
            align-items: center;
            color: #fff;
        }}

        .card {{
            background: rgba(255, 255, 255, 0.05);
            backdrop-filter: blur(12px);
            -webkit-backdrop-filter: blur(12px);
            padding: 3rem 4rem;
            border: 2px solid rgba(255, 255, 255, 0.2);
            border-radius: 16px;
            box-shadow: 0 20px 30px rgba(0,0,0,0.5);
            text-align: center;
            max-width: 600px;
        }}

        .card h1 {{
            font-size: 2.5rem;
            margin-bottom: 1rem;
            border-bottom: 3px solid #ffa0a0;
            padding-bottom: 1rem;
        }}

        .card p {{
            margin: 1rem 0;
            font-size: 1.1rem;
            color: #eaeaea;
        }}

        .link {{
            display: inline-block;
            margin-top: 2rem;
            padding: 0.75rem 1.5rem;
            font-size: 1rem;
            font-weight: bold;
            color: #000;
            background-color: #ffcc00;
            border-radius: 8px;
            text-decoration: none;
            transition: all 0.3s ease;
            box-shadow: 0 4px 10px rgba(0,0,0,0.3);
        }}

        .link:hover {{
            background-color: #ffc400;
            transform: scale(1.05);
        }}
    </style>
</head>
<body>
    <div class=""card"">
        <h1>{title}</h1>
        <p><strong>Status:</strong> Running</p>
        <p><strong>Time:</strong> {dateTime:G}</p>
        <a href=""{baseUrl}"" class=""link"">Open</a>
    </div>
</body>
</html>";
    }

}
