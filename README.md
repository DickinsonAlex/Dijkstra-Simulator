# Dijkstra Simulator

<!-- tags:start -->
![VB.NET](https://img.shields.io/badge/VB.NET-512BD4?logo=dotnet&logoColor=white) ![Algorithms](https://img.shields.io/badge/Algorithms-2F6F8F) ![Pathfinding](https://img.shields.io/badge/Pathfinding-2F6F8F) ![A-Level: Computer Science](https://img.shields.io/badge/A--Level-Computer%20Science-1F7A4D)
<!-- tags:end -->

A Windows Forms app for building a weighted graph and exploring **Dijkstra's shortest-path algorithm**. Made during my A-Level Computer Science course.

## Features

- Nodes are laid out evenly around a circle and labelled A, B, C…
- Click nodes to connect them and set edge weights
- A live **adjacency matrix** updates as you edit the graph
- Pick a start and end node and press **Calculate** to run **Dijkstra's algorithm**: the shortest route is highlighted in green on the graph and shown with its total distance (or you're told there's no path)
- Add and remove nodes, or clear the graph and start again
- The window and text scale with the form size

## Built with

- VB.NET, Windows Forms (.NET Core 3.1)
- Visual Studio

## Getting started

1. Open `Dijkstra Algorithm/Dijkstra Algorithm.sln` in Visual Studio.
2. Build and run (F5).

## Project structure

```text
Dijkstra Algorithm/
├── Form1.vb    Graph view: nodes, weights, the adjacency matrix and Dijkstra's shortest path
├── Form2.vb    Node management
└── Dijkstra Algorithm.sln / .vbproj
```

## How the search works

The graph is stored as an adjacency matrix, where `0` means "no edge". `GetPath_Click` in `Form1.vb` keeps a distance and a previous node for every vertex. It repeatedly settles the unvisited vertex with the smallest known distance and relaxes each of its edges, stopping as soon as the end node is settled. Following the previous-node links back from the end gives the route. With at most 15 nodes, a simple linear scan for the closest vertex is plenty; a priority queue would only matter for much bigger graphs.

## Credits

Made by Alex Dickinson.
