using Nekoyume.SingleClient.Models.TableData;
using Nekoyume.TableData;
using NUnit.Framework;

namespace Tests.EditMode.SingleClient.TableData
{
    public class BuffLimitRowViewMapperTest
    {
        [Test]
        public void ProjectsGroupIdAndValue()
        {
            var row = new BuffLimitSheet.Row();
            row.Set(new[] { "600001", "Percentage", "80" });

            var view = row.ToView();

            Assert.AreEqual(600001, view.GroupId);
            Assert.AreEqual(80, view.DurationLimit);
            // Value is currently mirrored into both fields; if csv later splits the column a
            // future mapper revision will populate them independently.
            Assert.AreEqual(80, view.DurationStack);
        }

        [Test]
        public void ToViewOnNullReturnsDefault()
        {
            BuffLimitSheet.Row row = null;
            Assert.AreEqual(default(BuffLimitRowView), row.ToView());
        }

        [Test]
        public void EqualityIsBasedOnGroupId()
        {
            var a = new BuffLimitSheet.Row();
            a.Set(new[] { "1", "Add", "100" });
            var b = new BuffLimitSheet.Row();
            b.Set(new[] { "1", "Percentage", "999" });

            Assert.AreEqual(a.ToView(), b.ToView());
        }
    }
}
