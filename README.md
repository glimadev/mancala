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

# Game rules

Each of the two players has his six pits in front of him. To the right of the six pits, each player
has a larger pit. At the start of the game, there are six stones in each of the six round pits.
The player who begins with the first move picks up all the stones in any of his own six pits,
and sows the stones on to the right, one in each of the following pits, including his own big
pit. No stones are put in the opponents' big pit. If the player's last stone lands in his own big
pit, he gets another turn. This can be repeated several times before it's the other player's
turn.

During the game the pits are emptied on both sides. Always when the last stone lands in an
own empty pit, the player captures his own stone and all stones in the opposite pit (the
other player’s pit) and puts them in his own (big or little?) pit.

The game is over as soon as one of the sides runs out of stones. The player who still has
stones in his pits keeps them and puts them in his big pit. The winner of the game is the
player who has the most stones in his big pit.