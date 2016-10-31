# ob-feed

## The Brief
Build a simple Xamarin application to pull an RSS feed, store on the device, and show a searchable front-end view of the feed. 

## The Process
### Design
The Mac design tool Sketch was used along with some studying of OB's corporate homepage to more accurately brand the app.

### Tech
Xamarin Forms is used primarily due to lack of previous exposure to the library, and the task presented itself as a decent opportunity to explore it.

Akavache is used for caching / keyvalue storage. Internally it uses SQL Lite 3, which while a little overkill, allows caching of URL string responses in a single line of code.

Microsoft's HttpClient is used for the networking aspect of the task, allowing again, a single line of code to download a string.

The above libs allow us to very quickly cache / download a string from a Url in a single line, which is incredibly useful when dealing solely with RSS feeds.
```
async Task<string> GetUrlString (string url) {
  // Get a cached string if it's < 1 hour old, else download a new string
  return await BlobCache.LocalMachine.GetOrFetchObject (url, () => new HttpClient ().GetStringAsync (url), DateTimeOffset.UtcNow.AddHours (1));
}
```

### Testing
There's pretty much like 10 lines of actual logic in this app. Some really short tests were added, but are merely to prove a point.
A UI Test library could be added, a bunch of the Xamarin evangelists suggest that a UI Test project should be added just to show that the app opens. Such a thing is as easy as File > New Project.

This app was deployed to iPhone SE 10.1 Simulator, iPad Pro (9.7 Inch) 10.1 Simulator, and a HTC Desire 300.

## The Verdict
Using Xamarin Forms is like telling a professional sculptor that he now has to use Lego to make his sculptures. Sure he can do things a little bit quicker once he gets to grips with Lego, and each piece is 'standard', but it sure as hell isn't going to look any good.

The brief was super basic, so the logic was expanded to allow any amount of feeds, provided the feed has a URL. This was proven by the added TechRadar default feed.

Honestly the most fun aspect of the task was the design of it, although using Xamarin Forms to translate that into an app proved difficult enough for me to consider redoing the app using Xamarin.Android / Xamarin.iOS which would have been quicker and looked/performed/felt better.
