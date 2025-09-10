using System.Text.RegularExpressions;

namespace NowPlayinObs.Client;

public class TemplateEngine
{
    private readonly Dictionary<string, Func<string>> _bindings = new();

    public void ResetBindings() => _bindings.Clear();

    public void Bind(string key, Func<string> getter)
        => _bindings[key] = getter;

    public string Render(string template)
    {
        return Regex.Replace(template, @"{{(.*?)}}", match =>
        {
            var key = match.Groups[1].Value.Trim();
            if (_bindings.TryGetValue(key, out var getter))
            {
                return getter();
            }
            return match.Value; // leave untouched if not found
        });
    }
}
