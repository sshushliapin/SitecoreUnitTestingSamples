using AutoFixture;
using AutoFixture.Xunit2;
using NSubstitute;
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
            Item parent = parentFake.WithChild(child1Fake).WithChild(child2Fake);
            Item child1 = child1Fake.WithName("Getting Started");
            Item child2 = child2Fake.WithName("Troubleshooting");

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
                    .Customize(new DatabaseCustomization()))
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
    }
}