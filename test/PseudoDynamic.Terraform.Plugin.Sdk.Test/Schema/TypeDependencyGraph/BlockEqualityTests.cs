﻿namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    public class BlockEqualityTests
    {
        public static IEnumerable<object[]> GetBlockSchemas()
        {
            yield return new object[] {
                typeof(Blocks.ZeroDepth),
                new BlockDefinition()
            };

            yield return new object[] {
                typeof(Blocks.HavingString),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("string", PrimitiveDefinition.String)
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.NestedBlock),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("block", new BlockDefinition() {
                                Attributes = new []{
                                    new BlockAttributeDefinition("string", PrimitiveDefinition.String)
                                }
                            })
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.TerraformValueNestedBlock),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("block", new BlockDefinition() {
                                Attributes = new []{
                                    new BlockAttributeDefinition("string", PrimitiveDefinition.String)
                                },
                                IsWrappedByTerraformValue = true
                            })
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.ListOfStrings),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("list", MonoRangeDefinition.List(PrimitiveDefinition.String))
                    }
                }
            };

            yield return new object[] {
                typeof(Blocks.MapOfObjects),
                new BlockDefinition() {
                    Attributes = new []{
                        new BlockAttributeDefinition("dictionary", new MapDefinition(new ObjectDefinition() {
                            Attributes = new []{
                                new ObjectAttributeDefinition("string", PrimitiveDefinition.String)
                            }
                        }))
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetBlockSchemas))]
        internal void Block_schema_matches_expected_block_schema(Type schemaType, TerraformDefinition expectedDefinition)
        {
            var actualDefinition = BlockBuilder.Default.BuildBlock(schemaType);
            Assert.Equal(expectedDefinition, actualDefinition, TerraformDefinitionEqualityComparer.Default);
        }

        public class Blocks
        {
            [Block]
            public class ZeroDepth { }

            [Block]
            public class HavingString
            {
                public string String { get; set; }
            }

            [Block]
            public class NestedBlock
            {
                [Block]
                public HavingString Block { get; set; }
            }

            [Block]
            public class TerraformValueNestedBlock
            {
                [Block]
                public ITerraformValue<HavingString> Block { get; set; }
            }

            [Block]
            public class ListOfStrings
            {
                public IList<string> List { get; set; }
            }

            [Block]
            public class MapOfObjects
            {
                public IDictionary<string, HavingString> Dictionary { get; set; }
            }
        }
    }
}