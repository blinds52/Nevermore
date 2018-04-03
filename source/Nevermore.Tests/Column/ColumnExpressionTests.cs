﻿using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Nevermore.Tests.Column
{
    public class ColumnExpressionTests
    {
        [Fact]
        public void ColumnExpression()
        {
            var actual = CreateQueryBuilder()
                .Column(c => c.Foo)
                .Column(c => c.Bar)
                .Column(c => c.Baz)
                .DebugViewRawQuery();

            actual.ShouldBeEquivalentTo(@"SELECT [Foo],
[Bar],
[Baz]
FROM dbo.[Records]
ORDER BY [Id]");
        }

        [Fact]
        public void ColumnExpressionWithColumnAlias()
        {
            var actual = CreateQueryBuilder()
                .Column(c => c.Foo, "A")
                .Column(c => c.Bar, "B")
                .Column(c => c.Baz, "C")
                .DebugViewRawQuery();

            actual.ShouldBeEquivalentTo(@"SELECT [Foo] AS [A],
[Bar] AS [B],
[Baz] AS [C]
FROM dbo.[Records]
ORDER BY [Id]");
        }

        [Fact]
        public void ColumnExpressionWithColumnAliasAndTableAlias()
        {
            const string tableAlias = "MyTable";
            var actual = CreateQueryBuilder()
                .Alias(tableAlias)
                .Column(c => c.Foo, "A", tableAlias)
                .Column(c => c.Bar, "B", tableAlias)
                .Column(c => c.Baz, "C", tableAlias)
                .DebugViewRawQuery();

            actual.ShouldBeEquivalentTo(@"SELECT MyTable.[Foo] AS [A],
MyTable.[Bar] AS [B],
MyTable.[Baz] AS [C]
FROM dbo.[Records] MyTable
ORDER BY [Id]");
        }

        static ITableSourceQueryBuilder<Record> CreateQueryBuilder()
        {
            return new TableSourceQueryBuilder<Record>("Records", 
                Substitute.For<IRelationalTransaction>(), 
                new TableAliasGenerator(), 
                new CommandParameterValues(),
                new Parameters(),
                new ParameterDefaults()
            );
        }

        class Record
        {
            public string Foo { get; }
            public int Bar { get; }
            public DateTime Baz { get; }
            public string Qux { get; }
        }
    }
}