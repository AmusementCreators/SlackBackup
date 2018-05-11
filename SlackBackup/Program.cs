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
            // ユーザデータの読み込み
            var str = System.IO.File.ReadAllText(Path.Combine(args[0], @"users.json"));
            var users = JsonConvert.DeserializeObject<User[]>(str).ToDictionary(u => u.Id);

            // チャンネルデータの読み込み
            str = System.IO.File.ReadAllText(Path.Combine(args[0], @"channels.json"));
            var channels = JsonConvert.DeserializeObject<Channel[]>(str).ToDictionary(u => u.Id);

			/*
            中身の形式は以下 
			CA3GL3EM7, accounting
            C04JEB1SH, altseed
            C1P9F83SM, archade
            */
            
#if false
			foreach(var c in channels)
			{
				Console.WriteLine($"{c.Key}, {c.Value.Name}");
			}
			foreach (var u in users)
            {
                Console.WriteLine($"{u.Key}, {u.Value.Name}");
            }

			return;
#endif

			// var regex = new Regex(@"<@(?<uid>[0-9A-Z]*?)|#(?<cid>[0-9A-Z]*?)\|?.*>");

            // チャンネルごとに、メッセージデータを読み込む
            foreach (var chName in channels.Select(ch => ch.Value.Name))
            {
                var path = Path.Combine(args[0], chName);
                var files = Directory.EnumerateFiles(path);
                var messages = new List<Message>();

                // メッセージは日付ごとに別々のファイルになっているので全部読み込む
                foreach (var f in files)
                {
                    str = System.IO.File.ReadAllText(f);
                    var m = JsonConvert.DeserializeObject<Message[]>(str);
                    Console.WriteLine($"{ f} Loaded...");
                    messages.AddRange(m);
                }

                // チャンネルごとにHTMLファイルに書き出し
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
                    foreach (var m in messages)
                    {
                        var user = (m.User != null && users.ContainsKey(m.User)) ? users[m.User] : null;
                        var uname = user?.Name ?? "unknown";
                        var date = DateTimeOffset.FromUnixTimeSeconds((long)double.Parse(m.Ts)).ToLocalTime().ToString();
                        var text = m.Text??"";
						// var r = regex.Matches(text);
						foreach(var c in channels)
						{
							var r = new Regex($@"<#{c.Key}\|?.*>");
							text = r.Replace(text, $"<a href=\"{c.Value.Name}.html\">#{c.Value.Name}</a>");
						}
						foreach (var u in users)
                        {
                            var r = new Regex($@"<@{u.Key}>");
							text = r.Replace(text, $"@{u.Value.Name}({u.Value.RealName})");
                        }

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
