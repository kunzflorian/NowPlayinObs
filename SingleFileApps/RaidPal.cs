#:package HtmlAgilityPack@1.12.4

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using HtmlAgilityPack;

var eventUrl = "https://raidpal.com/en/event/172-fryday-crew-dnb-raid-train-20-march-2026";
//eventUrl = "https://raidpal.com/en/event/172-fryday-crew-dnb-raid-train-13-march-2026";
var slotUrl = "https://raidpal.com/template/mods/mod_raidpal/assets/event_slots.php?id=";
var client = new HttpClient();
var doc = new HtmlDocument();

Console.WriteLine($"Reading from {eventUrl} [Enter] or provide:");
var buffer = Console.ReadLine()!;
if( buffer.StartsWith("http"))
    eventUrl = buffer;
// get the id
var response = await client.GetAsync(eventUrl);
if (!response.IsSuccessStatusCode)
    throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");

var html = await response.Content.ReadAsStringAsync();
doc.LoadHtml(html);
var eventNode = doc.DocumentNode.SelectSingleNode("//div[@id='rp__event']");

var eventId = eventNode.Attributes["data-event"].Value;

Console.WriteLine($"EventID: {eventId}");

slotUrl += eventId;

response = await client.GetAsync(slotUrl);
if (!response.IsSuccessStatusCode)
        throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");

html = await response.Content.ReadAsStringAsync();

doc.LoadHtml(html);

var container = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'rp_event_slots_loader')]");
//var container = doc.DocumentNode.SelectSingleNode("//div[@class='container-xl no-padding rp_event_slots_loader']");


var slots = container.SelectNodes("//div[contains(@class, 'row rp_slot_box')]");
List<Slot> results = [];

foreach (var slot in slots)
{
    // read time
    var cultureInfo = new CultureInfo("us-US");
    var margin = slot.SelectSingleNode("div[contains(@class, 'rp_slots_margin')]");
    var raw = margin.InnerText.Trim();
    var values = raw.Split("\n");
    var time = values[1].Trim().Split(' ')[0];
    var start = DateTime.Parse($"{values[0]} {time}", cultureInfo);
    // 
    var node = slot.SelectSingleNode("div/a");
    var artist = node is not null ? node.InnerText.Trim() : "[unknown]";
    Console.WriteLine($"{start} {artist}");
    results.Add(new(start, artist));
}

var json = JsonSerializer.Serialize(results, SlotContext.Default.ListSlot);
File.WriteAllText("lineup.json", json);


public record Slot(DateTime Start, string Streamer);

[JsonSerializable(typeof(Slot))]
[JsonSerializable(typeof(List<Slot>))]
internal partial class SlotContext : JsonSerializerContext { }