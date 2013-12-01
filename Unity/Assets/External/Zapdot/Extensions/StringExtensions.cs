using UnityEngine;
using System.Collections;

public static class StringExtensions
{
    public static string WordWrap(this string content, int maxCharsPerLine, int maxLines)
    {
        int lineCount = 0;

        string body = string.Empty;
        string[] lines = content.Trim().Replace("\r\n", "\n").Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (line.Length <= maxCharsPerLine)
            {
                body += line + "\n";
                lineCount++;
            }
            else
            {
                while (line.Length > 0)
                {
                    int endOfLineIndex = Mathf.Min(maxCharsPerLine, line.Length);
                    int spaceIndex = line.LastIndexOf(" ", endOfLineIndex);

                    if (endOfLineIndex < maxCharsPerLine || spaceIndex < 0)
                    {
                        spaceIndex = endOfLineIndex;
                    }

                    body += line.Substring(0, spaceIndex) + "\n";

                    lineCount++;

                    line = line.Substring(spaceIndex).Trim();
                }
            }
        }

        body = body.Trim();

        if (lineCount > maxLines)
        {
            Debug.LogError(string.Format("Message too long! Keep under {0} lines and {1} chars: {2}", maxLines, maxLines * maxCharsPerLine, body));
            body = body.Substring(0, Mathf.Min(maxLines * maxCharsPerLine, body.Length));
        }

        return body;
    }
}
