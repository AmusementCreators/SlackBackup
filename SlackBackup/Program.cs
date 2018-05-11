﻿using System;
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
						var unamereal = user?.RealName ?? uname;
                        var date = DateTimeOffset.FromUnixTimeSeconds((long)double.Parse(m.Ts)).ToLocalTime().ToString();
                        var text = m.Text??"";

                        // replace channel id to channel name
						foreach(var c in channels)
						{
							var r = new Regex($@"<#{c.Key}\|?.*>");
							text = r.Replace(text, $"<a href=\"{c.Value.Name}.html\">#{c.Value.Name}</a>");
						}
						// replace user id to user name
						foreach (var u in users)
                        {
                            var r = new Regex($@"<@{u.Key}>");
							text = r.Replace(text, $"{u.Value.RealName ?? u.Value.Name}(@{u.Value.Name})");
                        }
                        
						if(text.Contains("uploaded a file"))
						{
							var r = new Regex(@"<https:\/\/amusementcreators.slack.com\/files\/(?<url1>.+)\/(?<url2>.+)\/(?<url3>.+)\|(?<filename>.+)>");
							var match = r.Match(text);
							text = r.Replace(text, $"<a href=\"https://amusementcreators.slack.com/files/{match.Groups["url"]}/{match.Groups["ur2"]}/{match.Groups["ur3"]}\">{match.Groups["filename"]}</a>");
						}

                        writer.WriteLine($@"
<div class='message'>
    <span class='avater'>
        <img src='{user?.Profile?.Image48 ?? ""}' />
    </span>
    <span class='container'>
        <span class='header'>
            <span class='name'>
                {unamereal} (@{uname})
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
