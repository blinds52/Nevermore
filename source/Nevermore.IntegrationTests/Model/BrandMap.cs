﻿using System;
using System.Collections.Generic;
using Nevermore.Contracts;
using Nevermore.Mapping;
using Nevermore.Serialization;

namespace Nevermore.IntegrationTests.Model
{
    public class BrandMap : DocumentMap<Brand>
    {
        public BrandMap()
        {
            Column(m => m.Name);
        }
    }

    public class BrandConverter : InheritedClassConverter<Brand>
    {
        readonly Dictionary<string, Type> derivedTypeMappings = new Dictionary<string, Type>
        {
            {BrandA.BrandType, typeof(BrandA)},
            {BrandB.BrandType, typeof(BrandB)}
        };

        public BrandConverter(RelationalMappings relationalMappings) : base(relationalMappings)
        {
        }

        protected override IDictionary<string, Type> DerivedTypeMappings => derivedTypeMappings;
        protected override string TypeDesignatingPropertyName => "Type";
    }

    public class BrandToTestSerialization : IDocument
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string JSON { get; set; }
    }

    public class BrandToTestSerializationMap : DocumentMap<BrandToTestSerialization>
    {
        public BrandToTestSerializationMap()
        {
            TableName = "Brand";
            Column(x => x.Name);
            Column(x => x.JSON);
        }
    }

}