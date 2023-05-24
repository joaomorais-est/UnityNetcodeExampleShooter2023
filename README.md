Unity Netcode for GameObjects example shooter project (class 2023)
---

This is an academic project for a specific class (Videojogos Multijogador e em Rede) of the DVAM (Desenvolvimento de Videojogos e Aplicações Multimédia) CTeSP (Curso Técnico Superior Profissional) at Setúbal Polythecnic University - School of Technology.

It's meant to work as an example of the multiplayer features present in Unity Netcode for GameObjects (v1.2). The students started a Server in a class computer, and they all connected to that server on the other machines (Clients). The application was able to connect all players to the server and all clients could see each other and fire bullets. When some player was hit, they were informed that they had died.

It's (very) buggy, uncommented and (very) unoptimized.

Use it at your own risk ;-)

How to play
---

- Check the IP of the machine that's going to be the Server
- Change the IP in the NetworkManager GameObject, in the Unity Transport component (Connection Data > Address) to that IP
- Build the project.
- Run the build executable and start a Server on the machine that was selected to be the Server
- Run the build executable and start a Client on any other machine (inside the same network)