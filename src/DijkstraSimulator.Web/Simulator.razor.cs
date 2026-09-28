using DijkstraSimulator.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DijkstraSimulator.Web;

/// <summary>
/// The whole simulator page. The graph and the search both live in the shared
/// VB.NET library (DijkstraSimulator.Core); this component only draws them.
/// </summary>
public partial class Simulator : IDisposable
{
    private const int MinNodes = 2;
    private const int MaxNodes = 15;
    private const int MaxWeight = 50;

    // The SVG is drawn in a fixed 600 × 600 coordinate space and scaled by the browser
    private const double ViewSize = 600;
    private const double NodeRadius = 27;
    private const double LayoutRadius = ViewSize / 2 - NodeRadius - 40;

    private readonly Graph graph = new(6);
    private readonly Random random = new();

    private int selectedNode = -1;
    private int weight = 1;
    private int from;
    private int to = 5;

    private PathResult? result;
    private int position;              // which step is on screen; Steps.Count means the finished route
    private CancellationTokenSource? playback;

    private IReadOnlyList<(double X, double Y)> points = [];

    protected override void OnInitialized()
    {
        graph.Randomise(random, maxWeight: 12);
        graph.Changed += OnGraphChanged;
        UpdateLayout();
    }

    // ── Derived state ────────────────────────────────────────────────────

    private int NodeCount => graph.NodeCount;

    private bool IsStepping => result is not null && position < result.Steps.Count;

    private SearchStep? CurrentStep => IsStepping ? result!.Steps[position] : null;

    /// <summary>The table to show: the current step's, or the final one.</summary>
    private SearchStep? TableStep => result is null ? null : result.Steps[Math.Min(position, result.Steps.Count - 1)];

    private bool ShowRoute => result is { Found: true } && !IsStepping;

    private static string Name(int node) => Graph.NodeName(node);

    private static string FormatDistance(int distance) => distance == Dijkstra.Unreached ? "∞" : distance.ToString();

    private string Hint => selectedNode == -1
        ? "Click two nodes to join them with an edge. Click an edge to remove it."
        : $"Now click another node to join it to {Name(selectedNode)}, or click {Name(selectedNode)} again to cancel.";

    private string StepDescription
    {
        get
        {
            if (result is null) return "";
            var count = result.Steps.Count;
            if (!IsStepping)
                return $"Finished after settling {count} node{(count == 1 ? "" : "s")}. Replay the search to watch how it got there.";

            var step = result.Steps[position];
            var updates = step.Updated.Count == 0
                ? "no distances changed"
                : "updated " + string.Join(", ", step.Updated.Select(Name));
            return $"Step {position + 1} of {count}: settled {Name(step.Settled)} at distance {step.Distances[step.Settled]}; {updates}.";
        }
    }

    private string NodeClass(int node)
    {
        var classes = new List<string> { "node" };
        if (ShowRoute && result!.Route.Contains(node)) classes.Add("on-route");
        else if (CurrentStep is { } step)
        {
            if (node == step.Settled) classes.Add("current");
            else if (step.Visited[node]) classes.Add("settled");
        }
        if (node == selectedNode) classes.Add("selected");
        if (result is not null && node == result.Source) classes.Add("source");
        if (result is not null && node == result.Target) classes.Add("target");
        return string.Join(' ', classes);
    }

    private string EdgeClass(Edge edge)
    {
        if (ShowRoute && result!.UsesEdge(edge.A, edge.B)) return "edge on-route";
        if (CurrentStep is { } step &&
            ((edge.A == step.Settled && step.Updated.Contains(edge.B)) ||
             (edge.B == step.Settled && step.Updated.Contains(edge.A))))
            return "edge relaxed";
        return "edge";
    }

    /// <summary>Where to put a node's distance badge: just outside it, away from the centre.</summary>
    private (double X, double Y) BadgePosition(int node)
    {
        var (x, y) = points[node];
        var dx = x - ViewSize / 2;
        var dy = y - ViewSize / 2;
        var length = Math.Max(1, Math.Sqrt(dx * dx + dy * dy));
        var offset = NodeRadius + 22;
        return (x + dx / length * offset, y + dy / length * offset);
    }

    private static string F(double value) => value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);

    // ── Editing the graph ────────────────────────────────────────────────

    private void UpdateLayout() =>
        points = CircleLayout.Positions(graph.NodeCount, ViewSize / 2, ViewSize / 2, LayoutRadius);

    private void OnGraphChanged(object? sender, EventArgs e)
    {
        UpdateLayout();
        if (selectedNode >= graph.NodeCount) selectedNode = -1;
        from = Math.Min(from, graph.NodeCount - 1);
        to = Math.Min(to, graph.NodeCount - 1);
        ClearResult();
    }

    private void SetNodeCount(int count) => graph.Resize(Math.Clamp(count, MinNodes, MaxNodes));

    private void Randomise()
    {
        graph.Randomise(random, maxWeight: 12);
        from = 0;
        to = graph.NodeCount - 1;
    }

    private void ClickNode(int node)
    {
        if (selectedNode == -1)
            selectedNode = node;
        else if (selectedNode == node)
            selectedNode = -1;
        else
        {
            var first = selectedNode;
            selectedNode = -1;
            graph.SetEdge(first, node, Math.Clamp(weight, 1, MaxWeight));
        }
    }

    private void NodeKeyDown(KeyboardEventArgs e, int node)
    {
        if (e.Key is "Enter" or " ") ClickNode(node);
    }

    private void SetMatrixCell(int a, int b, ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var value) && value > 0)
            graph.SetEdge(a, b, Math.Min(value, 999));
        else if (string.IsNullOrWhiteSpace(e.Value?.ToString()) || value == 0)
            graph.RemoveEdge(a, b);
        // anything else is ignored and the next render puts the old value back
    }

    private void SetWeight(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var value))
            weight = Math.Clamp(value, 1, MaxWeight);
    }

    private void SetFrom(ChangeEventArgs e)
    {
        from = int.Parse(e.Value!.ToString()!);
        ClearResult();
    }

    private void SetTo(ChangeEventArgs e)
    {
        to = int.Parse(e.Value!.ToString()!);
        ClearResult();
    }

    // ── Running the search ───────────────────────────────────────────────

    private void FindPath()
    {
        StopPlayback();
        selectedNode = -1;
        result = Dijkstra.FindShortestPath(graph, from, to);
        position = result.Steps.Count;
    }

    private void ClearResult()
    {
        StopPlayback();
        result = null;
        position = 0;
    }

    private void GoTo(int newPosition)
    {
        StopPlayback();
        if (result is not null) position = Math.Clamp(newPosition, 0, result.Steps.Count);
    }

    private async Task Replay()
    {
        if (result is null) return;
        StopPlayback();
        var cts = playback = new CancellationTokenSource();
        position = 0;
        StateHasChanged();
        try
        {
            while (position < result.Steps.Count)
            {
                await Task.Delay(900, cts.Token);
                position++;
                StateHasChanged();
            }
        }
        catch (TaskCanceledException)
        {
            // stopped by the user or by an edit
        }
        finally
        {
            if (playback == cts) playback = null;
        }
    }

    private void StopPlayback()
    {
        playback?.Cancel();
        playback = null;
    }

    public void Dispose()
    {
        StopPlayback();
        graph.Changed -= OnGraphChanged;
    }
}
