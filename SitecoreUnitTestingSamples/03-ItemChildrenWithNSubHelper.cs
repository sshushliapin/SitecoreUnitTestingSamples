using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using NSubstitute;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.NSubstituteUtils;
using Xunit;

namespace SitecoreUnitTestingSamples
{
    public class ItemChildrenWithNSubHelper
    {
        [Theory, DefaultAutoData]
        public void ConfigureChildrenViaNSubstituteHelper(
            FakeItem parentFake,
            FakeItem child1Fake,
            FakeItem child2Fake)
        {
            child1Fake.WithName("Getting Started");
            child2Fake.WithName("Troubleshooting");

            var parent = (Item)parentFake.WithChild(child1Fake).WithChild(child2Fake);
            var child1 = (Item)child1Fake;
            var child2 = (Item)child2Fake;

            Assert.Same(child1, parent.Children["Getting Started"]);
            Assert.Same(child2, parent.Children["Troubleshooting"]);
            Assert.True(parent.HasChildren);
            Assert.Same(parent, child1.Parent);
            Assert.Same(parent, child2.Parent);
            Assert.Same(parent.ID, child1.ParentID);
            Assert.Same(parent.ID, child2.ParentID);
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
                item.HasChildren.Returns(c => item.Children.InnerChildren.Any());

                return item;
            }
        }
    }
}