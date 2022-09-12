# Mancala
Mancala is one of the oldest known two-player board games in the world, believed to have been created in ancient times. There is archeological and historical evidence that dates Mancala back to the year 700 AD in East Africa.

# Configuration
Number of pits and rocks can be configured in the appsettings
```
"MancalaConfig": {
    "Pits": 6,
    "Rocks": 4
  }
```

# How to run
If you have the .net 6 SDK installed you can just run the solution or otherwise with docker would be as below  
```
docker build -t mancala-image -f Mancala/Dockerfile .
docker run -d -p 8080:80 mancala-image
#Now you can open the browser with http://localhost:8080/index.html 
```


# Resources & Documentation
- [Adding docker](https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows)
- [Adding memory cache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-6.0)
- [Adding static files](https://dotnettutorials.net/lesson/wwwroot-folder-asp-net-core/#:~:text=Adding%20wwwroot%20(webroot)%20folder%20in,the%20folder%20name%20as%20wwwroot.)
- [Creating a custom filter](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-6.0#exception-filters)
- [Using configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0)
- [Youtube video about how to play in PT_BR](https://www.youtube.com/watch?v=zYiXGWqb3YY)
- [Board CSS Style](https://codepen.io/ChiliTomatoNoodle/pen/LOaPmy)

# Time to develop

16/18 hours