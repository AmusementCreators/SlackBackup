using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace SlackBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = System.IO.File.ReadAllText(Path.Combine(args[0], @"users.json"));
            var users = JsonConvert.DeserializeObject<User[]>(str).ToDictionary(u => u.Id);

            str = System.IO.File.ReadAllText(Path.Combine(args[0], @"channels.json"));
            var channels = JsonConvert.DeserializeObject<Channel[]>(str).ToDictionary(u => u.Id);

            //var regex = new Regex(@"<((?<uid>@.*?)|(?<cid>#.*?)\|.*?)>");


            foreach (var chName in channels.Select(ch => ch.Value.Name))
            {
                var path = Path.Combine(args[0], chName);
                var files = Directory.EnumerateFiles(path);
                var messages = new List<Message>();

                foreach (var f in files)
                {
                    str = System.IO.File.ReadAllText(f);
                    var m = JsonConvert.DeserializeObject<Message[]>(str);
                    Console.WriteLine($"{ f} Loaded...");
                    messages.AddRange(m);
                }

                using (var writer = new StreamWriter(Path.Combine(args[1], chName + ".html")))
                {
                    writer.WriteLine($@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>{chName}</title>
    <link rel='stylesheet' type='text/css' href='style.css' />
</head>
<body>
    

");
                    User currentUser = null;
                    foreach (var m in messages)
                    {
                        var user = (m.User != null && users.ContainsKey(m.User)) ? users[m.User] : null;
                        var uname = user?.Name ?? "unknown";
                        var date = DateTimeOffset.FromUnixTimeSeconds((long)double.Parse(m.Ts)).ToLocalTime().ToString();
                        var text = m.Text??"";
                        //var r = regex.Matches(text);

                        writer.WriteLine($@"
<div class='message'>
    <span class='avater'>
        <img src='{user?.Profile?.Image48 ?? ""}' />
    </span>
    <span class='container'>
        <span class='header'>
            <span class='name'>
                {uname}
            </span>
            <span class='date'>
                {date}
            </span>
        </span>
        <span class='text'>
            {text}
        </span>
    </span>
</div>
                        ");
                    }

                    writer.WriteLine(@"
</body>
</html>
                    ");
                }
            }
            return;

        }
    }
}
