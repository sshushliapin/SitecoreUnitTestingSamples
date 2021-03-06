﻿using AutoFixture;
using AutoFixture.Xunit2;
using NSubstitute;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Xunit;

namespace SitecoreUnitTestingSamples
{
    public class ItemChildren
    {
        [Theory, DefaultAutoData]
        public void GetChildren(
            Item root,
            Item child1,
            Item child2)
        {
            root.Children.InnerChildren.AddRange(new[] { child1, child2 });

            Assert.Same(child1, root.Children[child1.ID]);
            Assert.Same(child2, root.Children[child2.ID]);
        }

        [Theory, DefaultAutoData]
        public void GetChildrenByName(
            Item root,
            Item child1,
            Item child2)
        {
            child1.Name.Returns("Getting Started");
            child2.Name.Returns("Troubleshooting");

            root.Children.InnerChildren.AddRange(new[] { child1, child2 });

            Assert.Same(child1, root.Children["Getting Started"]);
            Assert.Same(child2, root.Children["Troubleshooting"]);
        }

        [Theory, DefaultAutoData]
        public void GetChildrenVsOtherProps(Item parent, Item child)
        {
            parent.Children.InnerChildren.AddRange(new[] { child });
            Assert.NotEmpty(parent.Children);
            // Assert.True(parent.HasChildren); // fails

            parent.HasChildren.Returns(true);
            Assert.True(parent.HasChildren); // pass
        }

        [Theory, DefaultAutoData]
        public void ChildrenParentProps(Item parent, Item child)
        {
            parent.Children.InnerChildren.AddRange(new[] { child });
            child.Parent.Returns(parent);
            child.ParentID.Returns(parent.ID);

            Assert.Same(parent, child.Parent);
            Assert.Same(parent.ID, child.ParentID);
        }

        internal class DefaultAutoDataAttribute : AutoDataAttribute
        {
            public DefaultAutoDataAttribute()
                : base(() => new Fixture()
                    .Customize(new DatabaseCustomization())
                    .Customize(new ItemCustomization()))
            {
            }
        }

        internal class DatabaseCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<Database>(x =>
                    x.FromFactory(() => Substitute.For<Database>())
                        .OmitAutoProperties());
            }
        }

        internal class ItemCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<Item>(x =>
                    x.FromFactory<ID, ItemData, Database>(CreateItem)
                        .OmitAutoProperties());
            }

            private static Item CreateItem(ID id, ItemData itemData, Database database)
            {
                var item = Substitute.For<Item>(id, itemData, database);
                item.Axes.Returns(Substitute.For<ItemAxes>(item));
                item.Children.Returns(Substitute.For<ChildList>(item, new ItemList()));
                return item;
            }
        }
    }
}