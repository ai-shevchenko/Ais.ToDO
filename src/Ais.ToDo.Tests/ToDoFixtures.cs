using Ais.ToDo.Core.Entities;

using AutoFixture;

namespace Ais.ToDo.Tests;

public static class ToDoFixtures
{
    public static IReadOnlyList<ToDoItem> GetItems(int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);

        var fixture = new Fixture();

        var items = Enumerable.Range(1, count)
            .Select(_ => fixture.Create<ToDoItem>())
            .ToList();

        return items;
    }
}