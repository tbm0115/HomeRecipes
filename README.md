This is an example of a Blazor WebAssembly project with PWA offline support. This site currently utilizes IndexedDB to sync recipe data from the server-side `recipes.json` with those
 that are saved locally within the browser for offline use.

# Libraries
The following libraries are particularly useful for this project:

 - [SIUnits](https://github.com/TrueAnalyticsSolutions/SIUnits): Provides humanization of standard units of measurement as well as easy conversion between factored units (ie. Cups to Tablespoons)
 - [Fractional.NET](https://github.com/smahjoub/Fractional.NET): Provides humanization of decimal values (ie. 1.25 cups isn't how recipes are normally noted)
 - [Schema.NET](https://github.com/RehanSaeed/Schema.NET): Used as a good baseline for which properties to map in the datamodel. This library bases its structure directly from [Schema.org](https://schema.org)

# Useful Links

 - [SteveSandersonMS/CarChecker](https://github.com/SteveSandersonMS/CarChecker): A great example of using IndexedDB in a Blazor WebAssembly app.

