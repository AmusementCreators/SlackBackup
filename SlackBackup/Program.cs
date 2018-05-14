using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace SlackBackup
{
    class Program
    {
        /// <summary>
        /// 太字にする部分を検出する正規表現
        /// </summary>
        static Regex regexBold = new Regex(@"(?>(^\*|\x20\*)(?<str>\w+?.*?)(\*\x20|\*$))", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// 消線にする部分を検出する正規表現
        /// </summary>
        static Regex regexLine = new Regex(@"(?>(^~|\x20~)(?<str>\w+?.*?)(~\x20|~$)|(^~|\x20~ )(?<str>\w+?.*)(~\x20|~$))", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// 斜体にする部分を検出する正規表現
        /// </summary>
        static Regex regexOblique = new Regex(@"(?>(^_|\x20_)(?<str>\w+?.*?)(_\x20|_$)|(^_|\x20_ )(?<str>\w+?.*)(_\x20|_$))", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// 添付ファイルURLを検出する正規表現
        /// </summary>
        static Regex regexAttachment = new Regex(@"(?><https:\/\/amusementcreators.slack.com\/files\/(?<url>.+)\|(?<filename>.+)>)", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// URLを検出する正規表現
        /// </summary>
        static Regex regexURL = new Regex(@"(?><http(?<url>.+)>)", RegexOptions.Compiled | RegexOptions.Multiline);

        static void Main(string[] args)
        {
            // ユーザデータの読み込み
            var str = System.IO.File.ReadAllText(Path.Combine(args[0], @"users.json"));
            var users = JsonConvert.DeserializeObject<User[]>(str).ToDictionary(u => u.Id);

            // チャンネルデータの読み込み
            str = System.IO.File.ReadAllText(Path.Combine(args[0], @"channels.json"));
            var channels = JsonConvert.DeserializeObject<Channel[]>(str).ToDictionary(u => u.Id);

            // チャンネルごとに、メッセージデータを読み込む
            Parallel.ForEach(channels.Select(ch => ch.Value.Name), chName =>
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
                        WriteHeader(writer, chName);
                        WriteSidebar(writer, channels, chName);

						writer.WriteLine("<div class='content'>");
                        foreach (var m in messages)
                        {
                            var user = (m.User != null && users.ContainsKey(m.User)) ? users[m.User] : null;
                            var uname = user?.Name ?? "unknown";
                            var unamereal = user?.RealName ?? uname;
                            var date = DateTimeOffset.FromUnixTimeSeconds((long)double.Parse(m.Ts)).ToLocalTime().ToString();
                            var text = m.Text ?? "";

                            // replace channel id to channel name
                            foreach (var c in channels)
                            {
                                var r = new Regex($@"<#{c.Key}\|?.*>");
                                text = r.Replace(text, $"<a href=\"{c.Value.Name}.html\">#{c.Value.Name}</a>");
                            }

                            // replace user id to user name
                            foreach (var u in users)
                            {
                                var r = new Regex($@"<@{u.Key}>");
                                text = r.Replace(text, $"@{u.Value.Name}({u.Value.RealName ?? u.Value.Name})");
                            }

                            // replace file url
                            if (text.Contains(@"<https:\/\/amusementcreators.slack.com\/files\/"))
                            {
                                var match = regexAttachment.Match(text);
                                var url = Regex.Replace($"{match.Groups["url"]}" ?? "", @"\/", @"/");
                                text = regexAttachment.Replace(text, $"<a href=\"https://amusementcreators.slack.com/files/{url}\">{match.Groups["filename"]}</a>");
                            }

                            // replace url
                            if (text.Contains(@"http"))
                            {
                                var match = regexURL.Match(text);
                                var url = Regex.Replace($"{match.Groups["url"]}" ?? "", @"\/", @"/");
                                text = regexURL.Replace(text, $"<a href=\"http{url}\">http{url}</a>");
                            }

                            // 文字修飾を反映
                            text = regexBold.Replace(text, @"<span class='bold'>${str}</span>");
                            text = regexOblique.Replace(text, @"<span class='oblique'>${str}</span>");
                            text = regexLine.Replace(text, @"<span class='line'>${str}</span>");

                            WriteMessage(writer, user, uname, unamereal, date, text);
                        }
						writer.WriteLine("</div>");
                        
                        WriteFooter(writer);
                    }
                });
            return;

        }

        /// <summary>
        /// HTMLヘッダを書き出す
        /// </summary>
        private static void WriteHeader(StreamWriter writer, string chName)
        {
            writer.WriteLine($@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>{chName} | AmusementCreators SlackBackup</title>
    <link rel='stylesheet' type='text/css' href='style.css' />
</head>
<body>    

");
        }

        /// <summary>
        /// サイドバーを書き出す
        /// </summary>
        private static void WriteSidebar(StreamWriter writer, Dictionary<string, Channel> channels, string chName)
        {
            writer.WriteLine("<div class='side_bar'>\r\n");
			writer.WriteLine(@"
    <span class='name'>channel</span>");
            foreach (var c in channels)
            {
                var text = $"<a href =\"{c.Value.Name}.html\">#{c.Value.Name}</a>\n";

                if (chName == c.Value.Name)
                {
                    writer.WriteLine($@"
    <span class='container' id='current'>
        {text}
    </span>
");
                }
                else
                {
                    writer.WriteLine($@"
    <span class='container'>
        {text}
    </span>
");
                }
            }
            writer.WriteLine("</div>\r\n");
        }

        /// <summary>
        /// メッセージを書き出す
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="user"></param>
        /// <param name="uname"></param>
        /// <param name="unamereal"></param>
        /// <param name="date"></param>
        /// <param name="text"></param>
        private static void WriteMessage(StreamWriter writer, User user, string uname, string unamereal, string date, string text)
        {
            writer.WriteLine($@"
<div class='message'>
    <span class='avater'>
        <img src='{user?.Profile?.Image48 ?? ""}' />
    </span>
    <span class='container'>
        <span class='header'>
            <span class='name'>
                @{uname} ({unamereal})
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

        /// <summary>
        /// HTMLフッタを書き出す
        /// </summary>
        /// <param name="writer"></param>
        private static void WriteFooter(StreamWriter writer)
        {
            writer.WriteLine(@"
</body>
</html>
");
        }
    }
}
