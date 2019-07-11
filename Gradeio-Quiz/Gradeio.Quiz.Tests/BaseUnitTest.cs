using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gradeio.Quiz.Tests
{
    public abstract class BaseUnitTest<TEntity>
    {
        public TEntity SystemUnderTest { get; set; }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            SetupMocking();
            SystemUnderTest = CreateSystemUnderTest();
        }

        public abstract TEntity CreateSystemUnderTest();

        public virtual void SetupMocking() { }
    }
}
